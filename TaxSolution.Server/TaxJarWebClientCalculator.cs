using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represnts and instance of a <see cref="ITaxCalculator"/> that uses an
    /// <see cref="WebClient"/> request mechanism.
    /// </summary>
    public class TaxJarWebClientCalculator : ITaxCalculator
    {   
        public TaxJarWebClientCalculator()
        {
            Console.WriteLine($"Instantiating {this.GetType()}");
            Console.WriteLine(Environment.NewLine);
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async ValueTask<TaxLocationRate> GetTaxRateByLocationAsync(TaxLocation location, CancellationToken token)
        {
            // Process request
            var zip = location.Zip;
            var country = location.Country;
            var uri = $@"https://api.taxjar.com/v2/rates/{zip}/?country={country}";
            using var wc = GetServiceConnection();
            string s = wc.DownloadString(uri);
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception("Rate data by location was empty.");
            Console.WriteLine(s);

            // Deserialize response
            var response = TaxJarHelper.DeserializeRateResponse(s);

            // Return location rates
            var locationRate = new TaxLocationRate
            {
                CityTax = response.CityRate,
                CombinedTax = response.CombinedRate,
                CountyTax = response.CountyRate,
                CountryTax = response.CountryRate,
                StateTax = response.StateRate
            };
            return await Task.FromResult(locationRate);
        }

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async ValueTask<decimal> GetTaxForOrderRequestAsync(TaxOrder order, CancellationToken token)
        {
            var jsonOrder = JsonSerializer.Serialize(order);
            var orderResponse = TaxJarHelper.ValidateOrderResponse(jsonOrder);

            return await GetTaxAmountAsync(orderResponse);
        }

        private async ValueTask<decimal> GetTaxAmountAsync(string orderResponse)
        {
            try
            {
                // Initialize request
                var uri = @"https://api.taxjar.com/v2/taxes";
                HttpWebRequest http = InitializeWebRequest(uri);

                // Prepare order payload
                var validatedJson = ValidateOrderResponse(orderResponse);

                // Transform into stream
                byte[] bytes = new ASCIIEncoding().GetBytes(validatedJson);
                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                // Read response content
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                using var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                // Extract rate value
                var jsonResult = JObject.Parse(content);
                var taxAmount = jsonResult.SelectToken("$.tax.amount_to_collect").ToString();
                return decimal.TryParse(taxAmount, out var rate)
                    ? await Task.FromResult(rate)
                    : throw new Exception($"Could not process rate value response: {taxAmount}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private HttpWebRequest InitializeWebRequest(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            var TOKEN = TaxJarHelper.GetToken();
            request.Headers.Add("Authorization", $"Bearer {TOKEN}");
            request.Headers.Add("x-api-version", "2020-08-07");
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = "POST";
            return request;
        }

        private string ValidateOrderResponse(string response)
        {
            // Verify that the order response contains valid json data
            var jsonResponse = JObject.Parse(response);
            var payload = @"{
  'from_country': '" + jsonResponse.SelectToken("$.from_country")?.ToString() + @"',
  'from_zip': '" + jsonResponse.SelectToken("$.from_zip")?.ToString() + @"',
  'from_state': '" + jsonResponse.SelectToken("$.from_state")?.ToString() + @"',
  'to_country': '" + jsonResponse.SelectToken("$.to_country")?.ToString() + @"',
  'to_zip': '" + jsonResponse.SelectToken("$.to_zip")?.ToString() + @"',
  'to_state': '" + jsonResponse.SelectToken("$.to_state")?.ToString() + @"',
  'amount': " + jsonResponse.SelectToken("$.amount")?.ToString() + @",
  'shipping': " + jsonResponse.SelectToken("$.shipping")?.ToString() + @",
  'line_items': " + jsonResponse.SelectToken("$.line_items") + @"
}";
            return payload.Replace("'", "\"");
        }

        private WebClient GetServiceConnection()
        {
            // Initialize client connection
            var wc = new WebClient();
            var TOKEN = TaxJarHelper.GetToken();
            wc.Headers.Add("Authorization", $"Bearer {TOKEN}");
            wc.Headers.Add("x-api-version", "2020-08-07");
            return wc;
        }
    }
}