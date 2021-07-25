namespace TaxSolution.Models
{
    /// <summary>
    /// Represents a tax location.
    /// </summary>
    public record TaxLocation
    {
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
    }
}