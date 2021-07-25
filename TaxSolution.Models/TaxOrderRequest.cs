namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a tax order request.
    /// </summary>
    public record TaxOrderRequest : BaseTaxRequest
    {
        public TaxOrder Order { get; set; }
    }
}