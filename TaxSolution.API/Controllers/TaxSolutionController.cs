using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Server;

namespace TaxSolution.API.Controllers
{
    /// <summary>
    /// Represents a service class for tax service api.
    /// <requirement>
    /// Your code test is to simply create a Tax Service that can take 
    /// a Tax Calculator in the class initialization and return the total tax 
    /// that needs to be collected.
    /// </requirement>
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TaxSolutionController : ControllerBase
    {
        private ITaxCalculator calculator;

        private void GetCalculator(string key)
        {
            calculator = TaxCalculatorFactory.GetCalculator(key);
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <param name="taxLocation"></param>
        /// <returns></returns>
        [HttpPost()]
        public async ValueTask<ActionResult<TaxLocationRate>> PostAsync(TaxLocationRequest request, CancellationToken token)
        {
            try
            {
                GetCalculator(request.CalcKey);
                return await calculator.GetTaxRateByLocationAsync(request.Location, token);
            }
            catch (TaskCanceledException tce)
            {
                Console.WriteLine($"Post execution cancelled: {tce.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPut()]
        public async ValueTask<ActionResult<decimal>> PutAsync(TaxOrderRequest request, CancellationToken token)
        {
            try
            {
                GetCalculator(request.CalcKey);
                return await calculator.GetTaxForOrderRequestAsync(request.Order, token);
            }
            catch (TaskCanceledException tce)
            {
                Console.WriteLine($"Put execution cancelled: {tce.Message}");
                throw;
            }
        }
    }
}
