using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represnts and instance of a <see cref="ITaxCalculator"/> that uses an
    /// <see cref="WebClient"/> request mechanism.
    /// </summary>
    public class TaxJarWebClientCalculator : TaxJarBaseClientCalculator, IGetServiceConnection<WebClient>
    {
        private readonly TaxJarConfiguration _taxConfig;
        private readonly ILogger _logger;

        public TaxJarWebClientCalculator(TaxJarConfiguration taxConfig,
            ILogger logger)
        {
            _taxConfig = taxConfig;
            _logger = logger;
            logger.LogInformation($"Instantiating {this.GetType()}");
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="request"></param>
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
            using var wc = GetServiceConnection();
            string s = wc.DownloadString(uri);
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
            try
            {
                // Initialize request
                var uri = @"https://api.taxjar.com/v2/taxes";
                HttpWebRequest http = InitializeWebRequest(uri);

                // Format order payload
                var formattedJson = jsonData?.Replace("'", "\"");
                if (formattedJson is null)
                {
                    throw new InvalidOperationException("Cannot read tax amount data.");
                }

                // Transform into stream
                byte[] bytes = new ASCIIEncoding().GetBytes(formattedJson);
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
                var taxAmount = jsonResult?.SelectToken("$.tax.amount_to_collect")?.ToString();
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
            var token = _taxConfig.Token;
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Headers.Add("x-api-version", "2020-08-07");
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Method = "POST";
            return request;
        }

        /// <summary>
        /// Gets the client service connection
        /// </summary>
        /// <returns></returns>
        public virtual WebClient GetServiceConnection()
        {
            // Initialize client connection
            var wc = new WebClient();
            var token = _taxConfig.Token;
            wc.Headers.Add("Authorization", $"Bearer {token}");
            wc.Headers.Add("x-api-version", "2020-08-07");
            return wc;
        }
    }
}