using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class AddItemPageViewModel : BaseViewModel, IQueryAttributable
    {
        private static readonly RestService RestClient = new RestService();
        private string _barcode;

        private bool _isVisibleMessageAndActivityIndicator;
        private string _name;
        private string _description;

        public AddItemPageViewModel()
        {
            AddItemCommand = new Command(async () =>
            {
                IsVisibleMessageAndActivityIndicator = true;

                if (await CreateItem())
                    IsVisibleMessageAndActivityIndicator = false;

                IsVisibleMessageAndActivityIndicator = false;

                await Task.Delay(TimeSpan.FromSeconds(0.5));

                var result = await Shell.Current.DisplayAlert("",
                    "Czy chcesz dodać więcej przedmiotów?",
                    "Nie",
                    "Tak");
                if (result)
                    await Shell.Current.GoToAsync("..");
            });

            GetBarcodeCommand = new Command(async () =>
            {
                var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
                var customScannerPage = new AddBarcodeScannerPage();
                await Shell.Current.Navigation.PushAsync(customScannerPage);
                this.IsBusy = true;

                customScannerPage.Disappearing += (sender, args) =>
                {
                    waitHandle.Set();
                };

                await Task.Run(() => waitHandle.WaitOne());

                // After the barcode was scanned...
                // BUG: After the PushAsync the display alerts launch
                var scannedBarcode = Preferences.Get("ScannedBarcode", null);

                if (string.IsNullOrEmpty(scannedBarcode))
                {
                    await Shell.Current.DisplayAlert("Niepowodzenie.", "Nie udało się zeskanować kodu kreskowego", "OK");
                    return;
                }

                await Shell.Current.DisplayAlert("Sukces!", "Pomyślnie zeskanowano kod kreskowy", "OK");
                Barcode = scannedBarcode;
                Preferences.Remove("ScannedBarcode");
                
                this.IsBusy = false;
            });
        }

        private Guid ItemId { get; } = new Guid();

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, nameof(Description));
        }

        private DateTimeOffset DateAdded { get; } = new DateTimeOffset(DateTimeOffset.Now.LocalDateTime);

        public string Barcode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value, nameof(Barcode));
        }

        public bool IsVisibleMessageAndActivityIndicator
        {
            get => _isVisibleMessageAndActivityIndicator;
            private set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value,
                nameof(IsVisibleMessageAndActivityIndicator));
        }

        public Command AddItemCommand { get; }

        public Command GetBarcodeCommand { get; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                var barcode = HttpUtility.UrlDecode(query?["Barcode"]);

                var foundBarcode = string.IsNullOrEmpty(barcode);

                Barcode = foundBarcode ? string.Empty : barcode;

            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine($"{this.GetType().Name}: Barcode not found!\nMeaning: Manual add or error with query\nError message:\n{e.Message}");
                Barcode = string.Empty;
            }
        }

        private async Task<bool> CreateItem()
        {
            var item = new Item(ItemId, Name, Description, Barcode, DateAdded);
            var result = await RestClient.AddItem(item);

            return result;
        }
    }
}