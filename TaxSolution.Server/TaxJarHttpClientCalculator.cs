using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represnts and instance of a <see cref="ITaxCalculator"/> that uses an
    /// <see cref="HttpClient"/> request mechanism.
    /// </summary>
    public class TaxJarHttpClientCalculator : TaxJarBaseClientCalculator, IGetServiceConnection<HttpClient>
    {
        private readonly TaxJarConfiguration _taxConfig;

        public TaxJarHttpClientCalculator(TaxJarConfiguration taxConfig)
        {
            _taxConfig = taxConfig;
            Console.WriteLine($"Instantiating {this.GetType()}");
            Console.WriteLine(Environment.NewLine);
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public override async ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync(TaxLocation? location, CancellationToken token)
        {
            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            // Process request
            var zip = location.Zip;
            var country = location.Country;
            var uri = $@"https://api.taxjar.com/v2/rates/{zip}/?country={country}";
            string s = await GetDataFromHttpClient(uri, token);
            if (string.IsNullOrWhiteSpace(s))
                throw new Exception("Rate data by location was empty.");
            Console.WriteLine(s);

            // Deserialize response
            var response = DeserializeRateResponse(s);
            
            // Return location rates
            var locationRate = new TaxLocationRate
            {
                CityTax = response?.CityRate,
                CombinedTax = response?.CombinedRate,
                CountyTax = response?.CountyRate,
                CountryTax = response?.CountryRate,
                StateTax = response?.StateRate
            };
            return await Task.FromResult(locationRate);
        }

        /// <summary>
        /// Gets tax amount from the provided Json data.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override async ValueTask<decimal> GetTaxAmountAsync(string? jsonData, CancellationToken token)
        {
            if (jsonData is null)
            {
                throw new ArgumentNullException(nameof(jsonData));
            }
            // Post order for tax amount
            var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var uri = @"https://api.taxjar.com/v2/taxes";
            var result = await PostDataFromHttpClient(uri, httpContent, token);

            // Process result and return tax amount
            var jsonResult = JObject.Parse(result.ToString());
            var amountValue = jsonResult?.SelectToken("$.tax.amount_to_collect")?.ToString();
            return decimal.TryParse(amountValue, out var amount)
                ? await Task.FromResult(amount)
                : throw new Exception($"Could not process rate value response: {amountValue}");
        }

        private async ValueTask<string> GetDataFromHttpClient(string uri, CancellationToken token)
        {
            // Process request and return content result
            using var hc = GetServiceConnection();
            var response = await hc.GetAsync(uri, token);
            return await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        }

        private async ValueTask<string> PostDataFromHttpClient(string uri, HttpContent payload, CancellationToken token)
        {
            // Process request and return content result
            if (payload == null)
                throw new Exception("HttpContent payload cannot be null.");
            using var hc = GetServiceConnection();
            var response = await hc.PostAsync(uri, payload, token);
            return await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the client service connection
        /// </summary>
        /// <returns></returns>
        public virtual HttpClient GetServiceConnection()
        {
            // Initialize client connection
            var hc = new HttpClient();
            var token = _taxConfig.Token; // TaxServiceConfiguration.GetToken();
            hc.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            hc.DefaultRequestHeaders.Add("x-api-version", "2020-08-07");
            return hc;
        }
    }
}