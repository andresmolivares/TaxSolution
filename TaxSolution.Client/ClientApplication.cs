using System;
using System.Threading.Tasks;
using TaxSolution.Models;

namespace TaxSolution.Client
{
    class ClientApplication
    {
        private readonly TaxSolutionViewModel vm;

        public ClientApplication(TaxSolutionViewModel _vm)
        {
            vm = _vm;
        }

        public async Task Run()
        {
            // Get tax rates for location - Flanders, NJ
            await RequestTaxRateForLocationAsync("07836");
            // Get generated tax order request
            var request = TaxSolutionViewModel.GenerateOrderRequest();
            await RequestTaxforOrderRequestAsync(request);
        }

        async Task RequestTaxRateForLocationAsync(string zip, string country = "US")
        {
            var request = new TaxLocationRequest
            {
                CalcKey = TaxSolutionViewModel.CALC_KEY,
                Location = new TaxLocation { Zip = zip, Country = country }
            };
            try
            {
                // Get the rates for the location
                var rates = await vm.GetTaxRateByLocationAsync(request);

                // Output to the client
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(@$"The tax rates for the location {request.Location.Zip}, {request.Location.Country} are {rates}");
                Console.WriteLine(Environment.NewLine);
                await Task.Yield();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} \nStack: {e.StackTrace}");
            }
        }

        async Task RequestTaxforOrderRequestAsync(TaxOrderRequest orderRequest)
        {
            try
            {
                // Get tax value for order
                var tax = await vm.GetTaxForOrderRequestAsync(orderRequest);

                // Output to the client
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(@$"The calculated tax is {tax:C} for the following order request {orderRequest}.");
                Console.WriteLine(Environment.NewLine);
                await Task.Yield();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} \nStack: {e.StackTrace}");
            }
        }
    }
}
