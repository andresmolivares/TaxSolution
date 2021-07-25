namespace TaxSolution.Models
{
    /// <summary>
    /// Base tax request that supports calculator key
    /// </summary>
    public abstract record BaseTaxRequest
    {
        public string CalcKey { get; set; }
    }
}
