using System.Linq;

namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a helper class to generate tax order requests.
    /// </summary>
    public static class TaxOrderRequestHelper
    {
        /// <summary>
        /// Returns a simulated tax order request.
        /// </summary>
        /// <returns></returns>
        public static TaxOrderRequest GetSimulatedTaxOrderRequest(string calcKey)
        {
            var order = new TaxOrder
            {
                from_country = "US",
                from_state = "CA",
                from_zip = "92093",
                to_country = "US",
                to_state = "CA",
                to_zip = "90002",
                shipping = 1.5m,
                line_items = new TaxOrderLineitem[] {
                    new TaxOrderLineitem { id = "1", quantity = 1, product_tax_code = "20010", unit_price = 15.75m, discount = 0 },
                    new TaxOrderLineitem { id = "2", quantity = 2, product_tax_code = "30010", unit_price = 15m, discount = 0 },
                    new TaxOrderLineitem { id = "3", quantity = 3, product_tax_code = "40010", unit_price = 15m, discount = 0 }
                }
            };
            order.amount = order.line_items.Sum(li => li.quantity * li.unit_price);
            return new TaxOrderRequest { CalcKey = calcKey, Order = order };
        }
    }
}