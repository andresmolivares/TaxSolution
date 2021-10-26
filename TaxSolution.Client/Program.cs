using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TaxSolution.Client
{
    /// <summary>
    /// Represents a class the runs the main client process.
    /// </summary>
    class Program
    {
        private static IServiceProvider? serviceProvider;

        static async Task Main(string[] args)
        {
            // Registering types into client container
            ConfigureServices();
            // Run client application
            if(serviceProvider is not null)
                await serviceProvider.GetRequiredService<ClientApplication>().Run();
            // Dispose
            DisposeServices();
            await Task.Yield();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddScoped<TaxSolutionViewModel>()
                .AddSingleton<ClientApplication>()
                ;
            serviceProvider = services.BuildServiceProvider();
            serviceProvider.CreateScope();
        }

        private static void DisposeServices()
        {
            if (serviceProvider is not null && serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
