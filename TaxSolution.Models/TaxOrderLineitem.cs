namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a tax order line item.
    /// </summary>
    public record TaxOrderLineitem
    {
        public string id { get; set; }
        public int quantity { get; set; }
        public string product_tax_code { get; set; }
        public decimal unit_price { get; set; }
        public decimal discount { get; set; }
    }
}