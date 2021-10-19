using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;

namespace TaxSolution.Client
{
    /// <summary>
    /// Represents a view model class for the client application.
    /// </summary>
    public class TaxSolutionViewModel
    {
        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="locationRequest"></param>
        /// <returns></returns>
        public async ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync(TaxLocationRequest? locationRequest)
        {
            // Initialize service request
            var uri = @"http://localhost:6700/api/taxsolution";
            var payload = JsonConvert.SerializeObject(locationRequest, Formatting.Indented);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
            
            // Process, deserialize and return response
            var results = await PostDataFromHttpClient(uri, httpContent);
            var taxLocationRate = JsonConvert.DeserializeObject<TaxLocationRate>(results);
            return taxLocationRate;
        }

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async ValueTask<decimal?> GetTaxForOrderRequestAsync(TaxOrderRequest? orderRequest)
        {
            // Initialize service request
            var uri = @"http://localhost:6700/api/taxsolution";
            var payload = JsonConvert.SerializeObject(orderRequest, Formatting.Indented);
            var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

            // Process, deserialize and return response
            var results = await PutDataFromHttpClient(uri, httpContent);
            return decimal.TryParse(results, out var taxAmount)
               ? taxAmount
               : throw new Exception("Error occurred parsing tax amount.");
        }

        private async ValueTask<string> PostDataFromHttpClient(string uri, HttpContent payload)
        {
            // Process request and return content result
            if (payload == null)
                throw new Exception("HttpContent payload cannot be null.");
            using var hc = GetServiceConnection();
            var response = await hc.PostAsync(uri, payload);
            return await response.Content.ReadAsStringAsync();
        }

        private async ValueTask<string> PutDataFromHttpClient(string uri, HttpContent payload)
        {
            // Process request and return content result
            if (payload == null)
                throw new Exception("HttpContent payload cannot be null.");
            using var hc = GetServiceConnection();
            var response = await hc.PutAsync(uri, payload);
            return await response.Content.ReadAsStringAsync();
        }

        private HttpClient GetServiceConnection()
        {
            // Initialize client connection
            return new HttpClient();
        }
    }
}
