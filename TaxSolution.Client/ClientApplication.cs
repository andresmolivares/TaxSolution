using System;
using System.Threading.Tasks;
using TaxSolution.Models.TaxLocation;
using TaxSolution.Models.TaxOrder;

namespace TaxSolution.Client
{
    class ClientApplication
    {
        private readonly TaxSolutionViewModel vm;

        public ClientApplication(TaxSolutionViewModel _vm)
        {
            vm = _vm;
        }

        /// <summary>
        /// Runs the <see cref="ClientApplication"/> app.
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            static string getCalculatorKey()
            {
                // NOTE: Also try calcKey values "ref" and "web"
                // for different calculator implementations
                return "http";
                //return "ref";
                //return "web";
            }

            // Get tax rates for location - Flanders, NJ
            await RequestTaxRateForLocationAsync(getCalculatorKey(), "07836");

            // Get generated tax order request
            await RequestTaxforOrderRequestAsync(getCalculatorKey());
        }

        async Task RequestTaxRateForLocationAsync(string calcKey, string zip, string country = "US")
        {
            // Create request
            var request = new TaxLocationRequest
            {
                CalcKey = calcKey,
                Location = new TaxLocation { Zip = zip, Country = country }
            };

            try
            {
                // Get the rates for the location
                var rates = await vm.GetTaxRateByLocationAsync(request);

                // Output to the client
                Console.WriteLine(@$"The tax rates for the location {request.Location.Zip}, {request.Location.Country} are {rates}");
                await Task.Yield();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} \nStack: {e.StackTrace}");
            }
        }

        async Task RequestTaxforOrderRequestAsync(string calcKey)
        {
            // Create request
            var orderRequest = TaxOrderRequestSimluator.GetSimulatedTaxOrderRequest(calcKey);

            try
            {
                // Get tax value for order
                var tax = await vm.GetTaxForOrderRequestAsync(orderRequest);

                // Output to the client
                Console.WriteLine(@$"The calculated tax is {tax:C} for the following order request {orderRequest}.");
                await Task.Yield();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} \nStack: {e.StackTrace}");
            }
        }
    }
}
