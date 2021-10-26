using System;


namespace TaxSolution.Desktop.Model
{
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
}


