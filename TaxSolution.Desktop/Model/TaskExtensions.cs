using System;
using System.Threading.Tasks;


namespace TaxSolution.Desktop.Model
{
    /// <summary>
    /// Utility that executes task and routes error to handler
    /// </summary>
    /// <source>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </source>
    public static class TaskExtensions
    {
        public static async Task FireAndForgetSafeAsync(this Task task, IErrorHandler? handler = null)
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


