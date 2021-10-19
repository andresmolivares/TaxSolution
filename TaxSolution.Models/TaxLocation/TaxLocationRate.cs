namespace TaxSolution.Models.TaxLocation
{
    /// <summary>
    /// Represents rates for a tax location.
    /// </summary>
    public record TaxLocationRate
    {
        public decimal? CityTax { get; set; }
        public decimal? CombinedTax { get; set; }
        public decimal? CountyTax { get; set; }
        public decimal? CountryTax { get; set; }
        public decimal? StateTax { get; set; }
        public override string ToString()
        {
            var output = @$"
CityTax: {CityTax}
CombinedTax: {CombinedTax}
CountyTax: {CountryTax}
CountryTax: {CountryTax}
StateTax: {StateTax}
";
            return output;
        }
    }
}
