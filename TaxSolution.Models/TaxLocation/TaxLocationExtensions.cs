using System;
using System.Linq;

namespace TaxSolution.Models.TaxLocation
{
    public static class TaxLocationExtensions
    {
        public static bool CheckZipCodeIsValid(this TaxLocation owner)
        {
            var zip = owner?.Zip;
            if (zip is null)
                throw new NullReferenceException("Could not access zip code on invalid tax location value.");
            // TODO: Add zip and country validation
            return zip.ToCharArray().Any(c => char.IsDigit(c));
        }
    }
}
