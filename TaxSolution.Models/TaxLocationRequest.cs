namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a tax order request
    /// </summary>
    public record TaxLocationRequest : BaseTaxRequest
    {
        public TaxLocation Location { get; set; }
    }
}