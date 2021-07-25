using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace TaxSolution.Client
{
    class Program
    {
        private static IServiceProvider serviceProvider;

        /// <summary>
        /// Main client process.
        /// </summary>
        /// <param name="args"></param>
        static async Task Main(string[] args)
        {
            // Registering types into client container
            ConfigureServices();
            // Run client application
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
            if (serviceProvider != null && serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
