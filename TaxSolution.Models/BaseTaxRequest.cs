namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a base tax request class that supports calculator key.
    /// </summary>
    public abstract record BaseTaxRequest
    {
        public string? CalcKey { get; set; }
    }
}
