using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Taxjar;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represnts and instance of a <see cref="ITaxCalculator"/> that uses an
    /// <see cref="TaxjarApi"/> client service reference request mechanism.
    /// </summary>
    public class TaxJarReferenceCalculator : ITaxCalculator, IGetServiceConnection<TaxjarApi>
    {
        private readonly TaxJarConfiguration _taxConfig;
        private readonly ILogger _logger;

        public TaxJarReferenceCalculator(TaxJarConfiguration taxConfig,
            ILogger logger)
        {
            _taxConfig = taxConfig;
            _logger = logger;
            logger.LogInformation($"Instantiating {this.GetType()}");
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync([Required] TaxLocation location, CancellationToken token)
        {
            if (location is null)
            {
                throw new ArgumentNullException(nameof(location));
            }
            // Process request
            var client = GetServiceConnection();
            // NOTE: Why does RatesForLocationAsync not support CancellationToken?
            var response = await client.RatesForLocationAsync(location.Zip, new
            {
                country = location.Country
            });
            // Return tax location rates
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
        public async ValueTask<decimal?> GetTaxForOrderRequestAsync([Required] TaxOrder order, CancellationToken token)
        {
            if (order is null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            // Get service ref and call API
            var client = GetServiceConnection();
            // Transform order request
            var orderRequest = order.AsOrderResponseAttributes();
            // NOTE: Why does TaxForOrderAsync not support CancellationToken?
            var taxResponse = await client.TaxForOrderAsync(orderRequest);
            // Get calculated order tax rate
            return await Task.FromResult(taxResponse.AmountToCollect);
        }

        public TaxjarApi GetServiceConnection()
        {
            return new TaxjarApi(_taxConfig.Token);
        }
    }

}