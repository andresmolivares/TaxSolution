using System;
using System.Threading.Tasks;
using TaxSolution.Client;
using TaxSolution.Models.TaxLocation;

namespace TaxSolution.Desktop.Model
{
    public class MainWindowViewModel : Notifiable
    {

        private readonly TaxSolutionViewModel vm;
        private IAsyncCommand<string>? _GetRatesCommand;
        private string? _ZipCode;
        private string? _RateDetails;

        public MainWindowViewModel()
        {
            vm = new TaxSolutionViewModel(); 
        }

        public string? ZipCode
        {
            get => _ZipCode;
            set
            {
                _ZipCode = value;
                NotifyChange(nameof(ZipCode));
                GetRatesCommand.RaiseCanExecuteChanged();
            }
        }

        public string? RateDetails
        {
            get => _RateDetails;
            set
            {
                _RateDetails = value;
                NotifyChange(nameof(RateDetails));
            }
        }

        public IAsyncCommand<string> GetRatesCommand
        {
            get => _GetRatesCommand ??= new AsyncCommand<string>(ExecuteGetRatesAsync, CanExecuteGetRates);
        }

        private bool CanExecuteGetRates(string? parameter)
        {
            return !string.IsNullOrWhiteSpace(parameter);
        }

        private async Task ExecuteGetRatesAsync(string parameter)
        {
            RateDetails = "Calculating...";
            var request = new TaxLocationRequest
            {
                CalcKey = "http",
                Location = new TaxLocation { Zip = parameter, Country = "US" }
            };
            try
            {
                if (!request.Location.CheckZipCodeIsValid())
                    throw new Exception("Invalid zip code value entered.");

                var details = await vm.GetTaxRateByLocationAsync(request);
                RateDetails = $"TaxJar Service results: \n{details}";
            }
            catch (Exception e)
            {
                RateDetails = $"Error occurred: {e.Message}";
            }
        }
    }
}
