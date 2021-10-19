using System;
using System.Threading.Tasks;
using System.Windows.Input;


namespace TaxSolution.Desktop.Model
{

    /// <summary>
    /// Base implementation for async command
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    /// <typeparam name="T"></typeparam>
    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler? CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task>? _execute;
        private readonly Func<T?, bool>? _canExecute;
        private readonly IErrorHandler? _errorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T?, bool>? canExecute = null, IErrorHandler? errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(T? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    if (_execute is null)
                        throw new NullReferenceException($"Command function cannot be null.");
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object? parameter)
        {
            _ = ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion Explicit implementations
    }
}


