namespace TaxSolution.Models.TaxLocation
{
    /// <summary>
    /// Represents a tax order request.
    /// </summary>
    public record TaxLocationRequest : BaseTaxRequest
    {
        public TaxLocation? Location { get; set; }
    }
}
