using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.Json;
using Taxjar;
using TaxSolution.Models;

namespace TaxSolution.Server
{
    /// <summary>
    /// Respresents a helper class for TaxJar calculator implementations.
    /// </summary>
    public static class TaxJarHelper
    {
        /// <summary>
        /// Returns the API token for the TaxJar service.
        /// TODO: This would typically go into a Configuration file.
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            return "5da2f821eee4035db4771edab942a4cc";
        }

        /// <summary>
        /// This validation logic is to ensure that we can use 
        /// an order retrived from the server. This function strips out the root
        /// node "order" and verifies values in the DOM.
        /// </summary>
        /// <param name="orderResponse"></param>
        /// <returns></returns>
        /// 
        public static string ValidateOrderResponse(string orderResponse)
        {
            // Validate shipping and amount value
            var parsedOrder = JObject.Parse(orderResponse).SelectToken("order")?.ToString();
            var rootNode = parsedOrder ?? orderResponse;
            var validatedJson = JObject.Parse(rootNode);
            var shippingValue = validatedJson["shipping"].ToString();
            validatedJson["shipping"] = decimal.TryParse(shippingValue, out var shipping)
                ? shipping
                : shippingValue;
            var amountValue = validatedJson["amount"].ToString();
            validatedJson["amount"] = decimal.TryParse(amountValue, out var amount)
                ? amount
                : amountValue;
            // Apply exemption type
            var exemptionTypeValue = validatedJson["exemption_type"].ToString();
            validatedJson["exemption_type"] = !string.IsNullOrWhiteSpace(exemptionTypeValue)
                ? exemptionTypeValue
                : "marketplace";
            // Validate both state and zip values
            if (!string.IsNullOrWhiteSpace(validatedJson["to_zip"].ToString())
                && !string.IsNullOrWhiteSpace(validatedJson["to_state"].ToString())
                && string.IsNullOrWhiteSpace(validatedJson["from_zip"]?.ToString())
                && string.IsNullOrWhiteSpace(validatedJson["from_state"]?.ToString()))
            {
                validatedJson["from_zip"] = validatedJson["to_zip"].ToString();
                validatedJson["from_state"] = validatedJson["to_state"].ToString();
            }
            // Validate zip value
            if (!string.IsNullOrWhiteSpace(validatedJson["to_zip"].ToString())
                && string.IsNullOrWhiteSpace(validatedJson["from_zip"]?.ToString()))
            {
                validatedJson["from_zip"] = validatedJson["to_zip"].ToString();
            }
            // Validate state value
            if (!string.IsNullOrWhiteSpace(validatedJson["to_state"].ToString())
                && string.IsNullOrWhiteSpace(validatedJson["from_state"]?.ToString()))
            {
                validatedJson["from_state"] = validatedJson["to_state"].ToString();
            }
            // Validate country value
            if (!string.IsNullOrWhiteSpace(validatedJson["to_country"].ToString())
                && string.IsNullOrWhiteSpace(validatedJson["from_country"]?.ToString()))
            {
                validatedJson["from_country"] = validatedJson["to_country"].ToString();
            }
            // Return the serialize validated json order
            return Newtonsoft.Json.JsonConvert.SerializeObject(validatedJson, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Deserializes and returns an instance of a <see cref="RateResponseAttributes"/>
        /// from the rateJsonResponse value.
        /// </summary>
        /// <param name="rateJsonResponse"></param>
        /// <returns></returns>
        public static RateResponseAttributes DeserializeRateResponse(string rateJsonResponse)
        {
            // NOTE: Fields names have "_"; need to remove
            var cleanJson = rateJsonResponse.Replace("_", string.Empty);
            // NOTE: Fields are retunred in a rate root node; need to extract
            var parsedJson = JObject.Parse(cleanJson).SelectToken("rate").ToString();
            //Console.WriteLine(parsedJson);
            // NOTE: During deserialization, numeric values are returned as strings. Adjusting serialization behavior with options.
            var deserialOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
            };

            var response = JsonSerializer.Deserialize<RateResponseAttributes>(parsedJson, deserialOptions);
            if (response == null)
                throw new Exception($"Encountered issues parsing rate response: {parsedJson}");
            return response;
        }

        /// <summary>
        /// Converts the <see cref="TaxOrderRequest"/> into an instance of
        /// an <see cref="OrderResponseAttributes"/> type.
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public static OrderResponseAttributes ConvertToOrderResponse(TaxOrder orderRequest)
        {
            if (orderRequest != null)
            {
                return new OrderResponseAttributes
                {
                    ExemptionType = orderRequest.exemption_type,
                    Amount = orderRequest.amount,
                    Shipping = orderRequest.shipping,
                    FromCountry = orderRequest.from_country,
                    FromState = orderRequest.from_state,
                    FromZip = orderRequest.from_zip,
                    ToCountry = orderRequest.to_country,
                    ToState = orderRequest.to_state,
                    ToZip = orderRequest.to_zip,
                    LineItems = orderRequest.line_items.Select(li =>
                       {
                           return new LineItem
                           {
                               Id = li.id,
                               Quantity = li.quantity,
                               ProductTaxCode = li.product_tax_code,
                               UnitPrice = li.unit_price,
                               Discount = li.discount
                           };
                       }).ToList()
                };
            }
            return null;
        }
    }
}