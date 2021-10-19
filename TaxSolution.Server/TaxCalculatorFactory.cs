using System;
using TaxSolution.Models;

namespace TaxSolution.Server
{
    /// <summary>
    /// Represents a class that provides tax calculator instances based on 
    /// key values.
    /// </summary>
    public static class TaxCalculatorFactory
    {
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
        public static ITaxCalculator GetCalculatorInstance(string? key, TaxJarConfiguration taxConfig) => 
            key?.ToLower() switch
        {
            "http" => new TaxJarHttpClientCalculator(taxConfig),
            "ref" => new TaxJarReferenceCalculator(taxConfig),
            "web" => new TaxJarWebClientCalculator(taxConfig),
            _ => throw new ArgumentException($"Invalid calculator key: {key}", nameof(key)),
        };
    }
}
