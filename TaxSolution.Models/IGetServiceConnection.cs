namespace TaxSolution.Models
{
    public interface IGetServiceConnection<TRestClient>
    {
        /// <summary>
        /// Returns the tax rate used for the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        TRestClient GetServiceConnection();
    }
}