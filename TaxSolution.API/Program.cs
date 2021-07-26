using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxSolution.API
{
    /// <summary>
    /// Represents a class the runs the main server process.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main server process.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates and configures a builder object.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(whb => whb.UseStartup<Startup>());
    }
}
