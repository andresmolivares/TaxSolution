using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TaxSolution.Models.TaxOrder
{
    /// <summary>
    /// Represents a tax 
    /// </summary>
    public record TaxOrder
    {
        [JsonPropertyName("FromLocation")]
        //[JsonConverter(typeof(TaxLocationConverter))]
        public TaxLocation.TaxLocation? FromLocation { get; set; }
        [JsonPropertyName("ToLocation")]
        //[JsonConverter(typeof(TaxLocationConverter))]
        public TaxLocation.TaxLocation? ToLocation { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; } = 0.00m;
        [JsonPropertyName("shipping")]
        public decimal Shipping { get; set; } = 0.00m;
        [JsonPropertyName("line_items")]
        public IEnumerable<TaxOrderLineitem>? LineItems { get; set; }
        [JsonPropertyName("exemption_type")]
        public string? ExemptionType { get; set; } = "marketplace";

        public void Validate()
        {
            if (FromLocation is not null)
            {
                if (!string.IsNullOrWhiteSpace(ToLocation?.Zip)
                && !string.IsNullOrWhiteSpace(ToLocation?.State)
                && string.IsNullOrWhiteSpace(FromLocation.Zip)
                && string.IsNullOrWhiteSpace(FromLocation.State))
                {
                    FromLocation.Zip = ToLocation?.Zip;
                    FromLocation.State = ToLocation?.State;
                }
                if (!string.IsNullOrWhiteSpace(ToLocation?.Zip)
                    && string.IsNullOrWhiteSpace(FromLocation.Zip))
                {
                    FromLocation.Zip = ToLocation?.Zip;
                }
                // Validate state value
                if (!string.IsNullOrWhiteSpace(ToLocation?.State)
                    && string.IsNullOrWhiteSpace(FromLocation.State))
                {
                    FromLocation.State = ToLocation?.State;
                }
                // Validate country value
                if (!string.IsNullOrWhiteSpace(ToLocation?.Country)
                    && string.IsNullOrWhiteSpace(FromLocation.Country))
                {
                    FromLocation.Country = ToLocation.Country;
                }
            }
        }

        public override string ToString()
        {
            var output = @$"
From Country: {FromLocation?.Country}
From Zip: {FromLocation?.Zip}
From State: {FromLocation?.State}
To Country: {ToLocation?.Country}
To Zip: {ToLocation?.Zip}
To State: {ToLocation?.State}
Amount: {Amount}
Shipping: {Shipping}
Exemption Type: {ExemptionType}
";
            return output;
        }
    }
 }
