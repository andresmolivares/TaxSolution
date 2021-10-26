using System;
using System.Linq;

namespace TaxSolution.Models.TaxLocation
{
    public static class TaxLocationExtensions
    {
        public static bool CheckZipCodeIsValid(this TaxLocation owner)
        {
            var zip = owner?.Zip;
            var valid = !string.IsNullOrWhiteSpace(zip)
                && zip.Replace("-", "").ToCharArray().All(c => char.IsDigit(c))
                && (zip.Replace("-", "").Length == 5 || zip.Replace("-", "").Length == 9);
            if (!valid)
                throw new InvalidOperationException("The zip code provided was in valid.");
            return valid;
        }
    }
}
