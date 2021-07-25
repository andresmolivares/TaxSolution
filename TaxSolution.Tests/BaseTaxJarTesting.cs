using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TaxSolution.Tests
{
    public class BaseTaxJarTesting
    {
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
    }
}
