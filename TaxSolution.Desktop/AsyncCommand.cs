﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;


namespace TaxSolution.Desktop
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

    /// <summary>
    /// Base implementation for async command
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    /// <typeparam name="T"></typeparam>
    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly IErrorHandler _errorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
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
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }
        #endregion
    }

    /// <summary>
    /// Interface for command error handling
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    /// <typeparam name="T"></typeparam>
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }


    /// <summary>
    /// Utility that executes task and routes error to handler
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}

