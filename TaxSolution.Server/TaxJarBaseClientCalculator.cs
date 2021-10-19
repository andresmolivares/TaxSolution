using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Taxjar;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;

namespace TaxSolution.Server
{
    public abstract class TaxJarBaseClientCalculator : ITaxCalculator
    {
        public abstract ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync(TaxLocation? location, CancellationToken token);

        public async ValueTask<decimal?> GetTaxForOrderRequestAsync(TaxOrder? order, CancellationToken token)
        {
            var jsonOrder = JsonSerializer.Serialize(order);
            var orderResponse = ValidateOrderResponse(jsonOrder);
            Console.WriteLine($"Validated Order from order request: {orderResponse}");

            return await GetTaxAmountAsync(orderResponse, token);
        }

        protected virtual async ValueTask<decimal> GetTaxAmountAsync(string? jsonData, CancellationToken token)
        {
            return await Task.FromResult(-0.01m);
        }

        /// <summary>
        /// This validation logic is to ensure that we can use 
        /// an order retrived from the server. This function strips out the root
        /// node "order" and verifies values in the DOM.
        /// </summary>
        /// <param name="orderResponse"></param>
        /// <returns></returns>
        /// 
        protected string? ValidateOrderResponse(string? orderResponse)
        {
            if (orderResponse is null)
            {
                throw new ArgumentNullException(nameof(orderResponse));
            }
            try
            {
                // Validate shipping and amount value
                var parsedOrder = JObject.Parse(orderResponse).SelectToken("order")?.ToString();
                var rootNode = parsedOrder ?? orderResponse;
                var validatedJson = JObject.Parse(rootNode);
                var shippingValue = validatedJson["shipping"]?.ToString();
                validatedJson["shipping"] = decimal.TryParse(shippingValue, out var shipping)
                    ? shipping
                    : shippingValue;
                var amountValue = validatedJson["amount"]?.ToString();
                validatedJson["amount"] = decimal.TryParse(amountValue, out var amount)
                    ? amount
                    : amountValue;
                // Apply exemption type
                var exemptionTypeValue = validatedJson["exemption_type"]?.ToString();
                validatedJson["exemption_type"] = !string.IsNullOrWhiteSpace(exemptionTypeValue)
                    ? exemptionTypeValue
                    : "marketplace";
                // Validate both state and zip values
                if (!string.IsNullOrWhiteSpace(validatedJson["to_zip"]?.ToString())
                    && !string.IsNullOrWhiteSpace(validatedJson["to_state"]?.ToString())
                    && string.IsNullOrWhiteSpace(validatedJson["from_zip"]?.ToString())
                    && string.IsNullOrWhiteSpace(validatedJson["from_state"]?.ToString()))
                {
                    validatedJson["from_zip"] = validatedJson["to_zip"]?.ToString();
                    validatedJson["from_state"] = validatedJson["to_state"]?.ToString();
                }
                // Validate zip value
                if (!string.IsNullOrWhiteSpace(validatedJson["to_zip"]?.ToString())
                    && string.IsNullOrWhiteSpace(validatedJson["from_zip"]?.ToString()))
                {
                    validatedJson["from_zip"] = validatedJson["to_zip"]?.ToString();
                }
                // Validate state value
                if (!string.IsNullOrWhiteSpace(validatedJson["to_state"]?.ToString())
                    && string.IsNullOrWhiteSpace(validatedJson["from_state"]?.ToString()))
                {
                    validatedJson["from_state"] = validatedJson["to_state"]?.ToString();
                }
                // Validate country value
                if (!string.IsNullOrWhiteSpace(validatedJson["to_country"]?.ToString())
                    && string.IsNullOrWhiteSpace(validatedJson["from_country"]?.ToString()))
                {
                    validatedJson["from_country"] = validatedJson["to_country"]?.ToString();
                }
                // Return the serialize validated json order
                return Newtonsoft.Json.JsonConvert.SerializeObject(validatedJson, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered issues validating order response.", e);
            }
        }

        /// <summary>
        /// Deserializes and returns an instance of a <see cref="RateResponseAttributes"/>
        /// from the rateJsonResponse value.
        /// </summary>
        /// <param name="rateJsonResponse"></param>
        /// <returns></returns>
        protected RateResponseAttributes? DeserializeRateResponse(string rateJsonResponse)
        {
            try
            {
                // NOTE: Fields names have "_"; need to remove
                var cleanJson = rateJsonResponse.Replace("_", string.Empty);
                // NOTE: Fields are retunred in a rate root node; need to extract
                var parsedJson = JObject.Parse(cleanJson)?.SelectToken("rate")?.ToString();
                if (parsedJson is null)
                {
                    throw new ApplicationException($"Error encountered parsing {nameof(rateJsonResponse)}.");
                }
                // NOTE: During deserialization, numeric values are returned as strings. Adjusting serialization behavior with options.
                var deserialOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
                };

                return JsonSerializer.Deserialize<RateResponseAttributes>(parsedJson, deserialOptions);
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered issues parsing rate response.", e);
            }
        }
    }
}
