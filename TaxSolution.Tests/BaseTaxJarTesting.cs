using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    /// <summary>
    /// Represents a base class used to exposed centralized test case data sources.
    /// </summary>
    public class BaseTaxJarTesting
    {
        /// <summary>
        /// Loads zip and country location data for tax rates test cases.
        /// </summary>
        /// <returns></returns>
        public static List<TestCaseData> LoadTaxRateByLocationTestCases()
        {
            return new[]
            {
                new TestCaseData("90002", "US"),
                new TestCaseData("10541", "US"),
                new TestCaseData("95008", "US"),
                new TestCaseData("M5V2T6", "CA"),
                new TestCaseData("32801", "US"),
                new TestCaseData("64155", "US"),
            }
            .ToList();
        }

        /// <summary>
        /// Loads calculator key data for calculator instantition test cases.
        /// </summary>
        /// <returns></returns>
        public static List<string> LoadCalcKeyTestCases()
        {
            return new[]
            {
                "http",
                "Web",
                "Ref",
            }
            .ToList();
        }

        protected virtual TaxJarConfiguration GetTaxConfig()
        {
            return new TaxJarConfiguration { Token = "5da2f821eee4035db4771edab942a4cc" };
        }

        protected virtual ITaxCalculatorFactory GetMockFactory()
        {
            var configInstance = GetTaxConfig();
            var configMock = new Mock<IOptionsMonitor<TaxJarConfiguration>>();
            configMock.Setup(o => o.CurrentValue).Returns(configInstance);
            return new TaxCalculatorFactory(configMock.Object, Mock.Of<ILogger>());
        }
    }
}
