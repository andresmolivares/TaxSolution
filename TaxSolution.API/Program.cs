using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxSolution.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(whb => whb.UseStartup<Startup>());
    }
}
