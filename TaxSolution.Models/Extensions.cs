using System.Linq;

namespace TaxSolution.Models
{
    public static class TaxLocationExtension
    {
        public static bool CheckZipCodeIsValid(this TaxLocation owner)
        {
            var zip = owner.Zip;
            // TODO: Add zip and country validation
            return zip.ToCharArray().Any(c => char.IsDigit(c));
        }

    }
}
