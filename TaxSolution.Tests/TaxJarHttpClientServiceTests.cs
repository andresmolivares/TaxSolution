using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    public class TaxJarHttpClientServiceTests : BaseTaxJarTesting
    {
        private ITaxCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = TaxCalculatorFactory.GetCalculatorInstance("http");
            Assert.IsInstanceOf<ITaxCalculator>(_calculator);
        }

        [TestCaseSource(nameof(LoadTaxRateByLocationTestCases))]
        [Test]
        public async Task GetTaxRateByLocationTest(string zip, string country)
        {
            var location = new TaxLocation { Zip = zip, Country = country };
            var rates = await _calculator.GetTaxRateByLocationAsync(location, CancellationToken.None);
            Console.WriteLine($"Rate are {rates}");
            Assert.IsTrue(rates.CityTax >= 0.0M);
            Assert.IsTrue(rates.CombinedTax >= 0.0M);
            Assert.IsTrue(rates.CountryTax >= 0.0M);
            Assert.IsTrue(rates.CountryTax >= 0.0M);
            Assert.IsTrue(rates.StateTax >= 0.0M);
            await Task.Yield();
        }

        [Test]
        public async Task GetTaxForOrderRequestTest()
        {
            const string key = "http";
            var request = TaxOrderRequestHelper.GetSimulatedTaxOrderRequest(key);
            var orderTax = await _calculator.GetTaxForOrderRequestAsync(request.Order, CancellationToken.None);
            Console.WriteLine($"Order tax for order request is {orderTax}");
            Assert.IsTrue(orderTax >= 0.0M);
            await Task.Yield();
        }
    }
}