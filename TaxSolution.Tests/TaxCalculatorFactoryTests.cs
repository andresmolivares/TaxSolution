using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    public class TaxCalculatorFactoryTests : BaseTaxJarTesting
    {
        protected override ITaxCalculatorFactory GetMockFactory()
        {
            var factory = base.GetMockFactory();
            Assert.IsInstanceOf<ITaxCalculatorFactory>(factory);
            return factory;
        }

        [TestCaseSource(nameof(LoadCalcKeyTestCases))]
        [Test]
        public async Task InstantiateCalculatorTest(string key)
        {
            var _taxCalculatorFactory = GetMockFactory();
            var calculator = _taxCalculatorFactory?.GetCalculatorInstance(key);
            Assert.IsInstanceOf<ITaxCalculator>(calculator);
            await Task.Yield();
        }

        [Test]
        public async Task InstantiateInvalidCalculatorTest()
        {
            var invalidKey = "invalid";
            Assert.Throws<InvalidOperationException>(() =>
            {
                var _taxCalculatorFactory = GetMockFactory();
                var calculator = _taxCalculatorFactory?.GetCalculatorInstance(invalidKey);
            });
            await Task.Yield();
        }
    }
}
