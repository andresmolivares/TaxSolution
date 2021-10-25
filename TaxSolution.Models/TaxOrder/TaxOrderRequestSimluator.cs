using System.Linq;

namespace TaxSolution.Models.TaxOrder
{
    /// <summary>
    /// Represents a helper class to generate tax order requests.
    /// </summary>
    public static class TaxOrderRequestSimluator
    {
        /// <summary>
        /// Returns a simulated tax order request.
        /// </summary>
        /// <returns></returns>
        public static TaxOrderRequest GetSimulatedTaxOrderRequest(string? calcKey)
        {
            var order = new TaxOrder
            {
                FromLocation = new TaxLocation.TaxLocation { Country = "US", State = "CA", Zip = "92093" },
                ToLocation = new TaxLocation.TaxLocation { Country = "US", State = "CA", Zip = "90002" },
                Shipping = 1.5m,
                LineItems = new TaxOrderLineitem[] {
                    new TaxOrderLineitem { id = "1", quantity = 1, product_tax_code = "20010", unit_price = 15.75m, discount = 0 },
                    new TaxOrderLineitem { id = "2", quantity = 2, product_tax_code = "30010", unit_price = 15m, discount = 0 },
                    new TaxOrderLineitem { id = "3", quantity = 3, product_tax_code = "40010", unit_price = 15m, discount = 0 }
                }
            };
            order.Amount = order.LineItems.Sum(li => li.quantity * li.unit_price);
            return new TaxOrderRequest { CalcKey = calcKey, Order = order };
        }
    }
}
