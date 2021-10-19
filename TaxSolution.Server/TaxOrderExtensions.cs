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
                    ExemptionType = orderRequest.exemption_type,
                    Amount = orderRequest.amount,
                    Shipping = orderRequest.shipping,
                    FromCountry = orderRequest.from_country,
                    FromState = orderRequest.from_state,
                    FromZip = orderRequest.from_zip,
                    ToCountry = orderRequest.to_country,
                    ToState = orderRequest.to_state,
                    ToZip = orderRequest.to_zip,
                    LineItems = orderRequest.line_items?.Select(li =>
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