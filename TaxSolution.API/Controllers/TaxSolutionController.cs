using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;
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
        private readonly ITaxCalculatorFactory _factory;
        private readonly ILogger _logger;

        public TaxSolutionController(ITaxCalculatorFactory factory, 
            ILogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        private ITaxCalculator GetCalculator(string? key)
        {
            return _factory.GetCalculatorInstance(key);
        }

        /// <summary>
        /// Returns the tax rate for the specified location.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        /// <returns></returns>
        [HttpPost()]
        public async ValueTask<ActionResult<TaxLocationRate?>> PostAsync([Required]TaxLocationRequest request, CancellationToken token)
        {
            if (request is null || request.Location is null)
            {
                var error = $"{nameof(request)} parameter or its Location cannot be null.";
                _logger.LogError(error);
                throw new ArgumentNullException(nameof(request));
            }
            try
            {
                var calculator = GetCalculator(request.CalcKey);
                if (calculator is null)
                {
                    var error = $"{nameof(calculator)} was not properly initialized.";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }
                var result = await calculator.GetTaxRateByLocationAsync(request.Location, token).ConfigureAwait(false);
                return Ok(result);
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogError($"Post execution cancelled: {tce.Message}");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError($"Post execution exception occurred: {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        /// <returns></returns>
        [HttpPut()]
        public async ValueTask<ActionResult<decimal?>> PutAsync([Required] TaxOrderRequest request, CancellationToken token)
        {
            if (request is null || request.Order is null)
            {
                var error = $"{nameof(request)} parameter or its Order cannot be null.";
                _logger.LogError(error);
                throw new ArgumentNullException(nameof(request));
            }
            try
            {
                var calculator = GetCalculator(request.CalcKey);
                if (calculator is null)
                {
                    var error = $"{nameof(calculator)} was not properly initialized.";
                    _logger.LogError(error);
                    throw new InvalidOperationException(error);
                }
                var result = await calculator.GetTaxForOrderRequestAsync(request.Order, token).ConfigureAwait(false);
                return Ok(result);
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogError($"Put execution cancelled: {tce.Message}");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError($"Put execution exception occurred: {e.Message}");
                throw;
            }
        }
    }
}

