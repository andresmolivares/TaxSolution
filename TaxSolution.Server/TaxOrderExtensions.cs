using System.Linq;
using Taxjar;
using TaxSolution.Models.TaxOrder;

namespace TaxSolution.Server
{
    public static class TaxOrderExtensions
    {
        /// <summary>
        /// Retursn an <see cref="OrderResponseAttributes"/> instance converted from the
        /// owning <see cref="TaxOrderRequest"/> .
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        public static OrderResponseAttributes? AsOrderResponseAttributes(this TaxOrder orderRequest)
        {
            if (orderRequest is not null)
            {
                return new OrderResponseAttributes
                {
                    ExemptionType = orderRequest.ExemptionType,
                    Amount = orderRequest.Amount,
                    Shipping = orderRequest.Shipping,
                    FromCountry = orderRequest.FromLocation?.Country,
                    FromState = orderRequest.FromLocation?.State,
                    FromZip = orderRequest.FromLocation?.Zip,
                    ToCountry = orderRequest.ToLocation?.Country,
                    ToState = orderRequest.ToLocation?.State,
                    ToZip = orderRequest.ToLocation?.Zip,
                    LineItems = orderRequest.LineItems?.Select(li =>
                    {
                        return new LineItem
                        {
                            Id = li.id,
                            Quantity = li.quantity,
                            ProductTaxCode = li.product_tax_code,
                            UnitPrice = li.unit_price,
                            Discount = li.discount
                        };
                    }).ToList()
                };
            }
            return null;
        }
    }
}