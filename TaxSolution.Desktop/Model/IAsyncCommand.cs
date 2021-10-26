using System.Threading.Tasks;
using System.Windows.Input;


namespace TaxSolution.Desktop.Model
{
    /// <summary>
    /// Interface for async command
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    /// <typeparam name="T"></typeparam>
    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
        // NOTE: Exposed for client implementation
        void RaiseCanExecuteChanged();
    }
}


