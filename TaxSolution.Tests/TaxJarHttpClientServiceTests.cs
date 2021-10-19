using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    public class TaxJarHttpClientServiceTests : BaseTaxJarTesting
    {
        private ITaxCalculator? _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = TaxCalculatorFactory.GetCalculatorInstance("http", GetTaxConfig());
            Assert.IsInstanceOf<ITaxCalculator>(_calculator);
        }

        [TestCaseSource(nameof(LoadTaxRateByLocationTestCases))]
        [Test]
        public async Task GetTaxRateByLocationTest(string zip, string country)
        {
            var location = new TaxLocation { Zip = zip, Country = country };
            if (_calculator is null)
            {
                Assert.Fail($"{nameof(_calculator)} cannot be null for the {nameof(GetTaxRateByLocationTest)} test.");
                Assert.IsNotNull(_calculator);
            }
            else
            {
                var rates = await _calculator.GetTaxRateByLocationAsync(location, CancellationToken.None).ConfigureAwait(false);
                Assert.IsNotNull(rates);
                Console.WriteLine($"Rates are {rates}");
                Assert.IsTrue(rates?.CityTax >= 0.0M);
                Assert.IsTrue(rates?.CombinedTax >= 0.0M);
                Assert.IsTrue(rates?.CountryTax >= 0.0M);
                Assert.IsTrue(rates?.CountryTax >= 0.0M);
                Assert.IsTrue(rates?.StateTax >= 0.0M);
            }
            await Task.Yield();
        }

        [Test]
        public async Task GetTaxForOrderRequestTest()
        {
            const string key = "http";
            var request = TaxOrderRequestSimluator.GetSimulatedTaxOrderRequest(key);
            if (_calculator is null)
            {
                Assert.Fail($"{nameof(_calculator)} cannot be null for the {nameof(GetTaxForOrderRequestTest)} test.");
                Assert.IsNotNull(_calculator);
            }
            else
            {
                var orderTax = await _calculator.GetTaxForOrderRequestAsync(request?.Order, CancellationToken.None).ConfigureAwait(false);
                Assert.IsNotNull(orderTax);
                Console.WriteLine($"Order tax for order request is {orderTax}");
                Assert.IsTrue(orderTax >= 0.0M);
            }
            await Task.Yield();
        }
    }
}