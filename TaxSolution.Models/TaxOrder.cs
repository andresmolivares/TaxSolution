using System.Collections.Generic;

namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a tax order.
    /// </summary>
    public record TaxOrder
    {
        public string from_country { get; set; }
        public string from_zip { get; set; }
        public string from_state { get; set; }
        public string to_country { get; set; }
        public string to_zip { get; set; }
        public string to_state { get; set; }
        public decimal amount { get; set; }
        public decimal shipping { get; set; }
        public IEnumerable<TaxOrderLineitem> line_items { get; set; }
        public string exemption_type { get; set; } = "marketplace";
        public override string ToString()
        {
            var output = @$"
From Country: {from_country}
From Zip: {from_zip}
From State: {from_state}
To Country: {to_country}
To Zip: {to_zip}
To State: {to_state}
Amount: {amount}
Shipping: {shipping}
Exemption Type: {exemption_type}
";
            return output;
        }
    }
}