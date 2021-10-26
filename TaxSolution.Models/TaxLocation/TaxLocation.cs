using System.Text.Json.Serialization;

namespace TaxSolution.Models.TaxLocation
{
    /// <summary>
    /// Represents a tax location.
    /// </summary>
    public record TaxLocation
    {
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("country")]
        public string? Country { get; set; }
        [JsonPropertyName("zip")]
        public string? Zip { get; set; }
    }
}
