using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TaxSolution.Models.TaxLocation;

namespace TaxSolution.Models
{
    /// <summary>
    /// Represents an interface for a tax calculator.
    /// <requirement>
    /// There are a lot of Tax calculation API’s out there and we need to be able 
    /// to work with many of them via a common interface we define in a service class.
    /// </requirement>
    /// </summary>
    public interface ITaxCalculator
    {
        /// <summary>
        /// Returns the tax rate used for the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        ValueTask<TaxLocationRate?> GetTaxRateByLocationAsync([Required]TaxLocation.TaxLocation location, CancellationToken token);

        /// <summary>
        /// Returns the tax amount for the specified order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        ValueTask<decimal?> GetTaxForOrderRequestAsync([Required] TaxOrder.TaxOrder order, CancellationToken token);
    }
}