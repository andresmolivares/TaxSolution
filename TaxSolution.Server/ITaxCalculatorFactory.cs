using TaxSolution.Models;

namespace TaxSolution.Server
{
    public interface ITaxCalculatorFactory
    {
        ITaxCalculator GetCalculatorInstance(string? key);
    }
}