using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.DataAnnotations;
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
        /// <summary>
        /// Returns the tax rate used for the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync([Required] TaxLocation location, CancellationToken token);

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual async ValueTask<decimal?> GetTaxForOrderRequestAsync([Required] TaxOrder order, CancellationToken token)
        {
            var orderResponse = ValidateOrderResponse(order);
            Console.WriteLine($"Validated Order from order request: {orderResponse}");

            return await GetTaxAmountAsync(orderResponse, token);
        }

        /// <summary>
        /// Gets tax amount from the provided Json data.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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
        protected string? ValidateOrderResponse(TaxOrder? order)
        {
            order?.Validate();
            try
            {
                var result = @"
{
    'shipping': " + order?.Shipping + @",
    'amount': " + order?.Amount + @",
    'exemption_type': '" + order?.ExemptionType + @"',
    'to_zip': '" + order?.ToLocation?.Zip + @"',
    'to_state': '" + order?.ToLocation?.State + @"',
    'to_country': '" + order?.ToLocation?.Country + @"',
    'from_zip': '" + order?.FromLocation?.Zip + @"',
    'from_state': '" + order?.FromLocation?.State + @"',
    'from_country': '" + order?.FromLocation?.Country + @"',
    'line_items': " + Newtonsoft.Json.JsonConvert.SerializeObject(order?.LineItems) + @"
}";

                return result;
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
