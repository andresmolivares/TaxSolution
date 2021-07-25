using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    public class TaxCalxulatorFactoryTests : BaseTaxJarTesting
    {
        [TestCaseSource(nameof(LoadCalcKeyTestCases))]
        [Test]
        public async Task InstantiateCalculatorTest(string key)
        {
            var calculator = TaxCalculatorFactory.GetCalculator(key);
            Assert.IsInstanceOf<ITaxCalculator>(calculator);
            await Task.Yield();
        }

        [Test]
        public async Task InstantiateInvalidCalculatorTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var key = "invalid";
                var calculator = TaxCalculatorFactory.GetCalculator(key);
            });
            await Task.Yield();
        }
    }
}
