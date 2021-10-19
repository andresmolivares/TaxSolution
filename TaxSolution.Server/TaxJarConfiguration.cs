using System.Configuration;

namespace TaxSolution.Server
{
    /// <summary>
    /// Respresents a helper class for TaxJar calculator implementations.
    /// </summary>
    public class TaxJarConfiguration
    {
       /// <summary>
       /// Gets and sets the token
       /// </summary>
        public string? Token { get; set; }
    }
}