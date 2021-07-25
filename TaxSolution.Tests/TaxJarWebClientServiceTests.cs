﻿using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Server;

namespace TaxSolution.Tests
{
    public class TaxJarWebClientServiceTests : BaseTaxJarTesting
    {
        private ITaxCalculator _calculator;

        [SetUp]
        public void Setup()
        {
            _calculator = TaxCalculatorFactory.GetCalculator("web");
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
        public async Task GetTaxForOrderTest()
        {
            const string key = "web";
            var request = TaxOrderRequestHelper.SimulateOrder(key);
            var orderTax = await _calculator.GetTaxForOrderRequestAsync(request.Order, CancellationToken.None);
            Console.WriteLine($"Order tax for order request is {orderTax}");
            Assert.IsTrue(orderTax >= 0.0M);
            await Task.Yield();
        }
    }
}