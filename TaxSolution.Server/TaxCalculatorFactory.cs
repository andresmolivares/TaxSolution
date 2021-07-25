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
        /// Gets instance of calculator based on key. 
        /// </summary>
        /// <requiement>
        /// Eventually we would have several Tax Calculators and 
        /// the Tax Service would need to decide which to use 
        /// based on the Customer that is consuming the Tax Service.
        /// </requiement>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ITaxCalculator GetCalculator(string key) => key.ToLower() switch
        {
            "http" => new TaxJarHttpClientCalculator(),
            "ref" => new TaxJarReferenceCalculator(),
            "web" => new TaxJarWebClientCalculator(),
            _ => throw new ArgumentException($"Invalid calculator key: {key}", nameof(key)),
        };
    }
}
