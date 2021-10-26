using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TaxSolution.Models;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represents a class that provides tax calculator instances based on 
    /// key values.
    /// </summary>
    public class TaxCalculatorFactory : ITaxCalculatorFactory
    {
        private readonly TaxJarConfiguration _taxConfig;
        private readonly ILogger _logger;

        public TaxCalculatorFactory(
            IOptionsMonitor<TaxJarConfiguration> taxConfig,
            ILogger logger
            )
        {
            _taxConfig = taxConfig.CurrentValue;
            _logger = logger;
        }
        /// <summary>
        /// Gets instance of calculator in the TaxSolution.Server 
        /// library based on the specified key.
        /// </summary>
        /// <requiement>
        /// Eventually we would have several Tax Calculators and 
        /// the Tax Service would need to decide which to use 
        /// based on the Customer that is consuming the Tax Service.
        /// </requiement>
        /// <param name="key"></param>
        /// <returns></returns>
        public ITaxCalculator GetCalculatorInstance(string? key) =>
            key?.ToLower() switch
            {
                "http" => new TaxJarHttpClientCalculator(_taxConfig, _logger),
                "ref" => new TaxJarReferenceCalculator(_taxConfig, _logger),
                "web" => new TaxJarWebClientCalculator(_taxConfig, _logger),
                _ => throw new InvalidOperationException($"Invalid calculator key: {key}"),
            };
    }
}
