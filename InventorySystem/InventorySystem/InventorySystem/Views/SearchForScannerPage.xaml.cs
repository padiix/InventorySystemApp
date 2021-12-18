using System;
using System.Collections.Generic;
using InventorySystem.Models;
using InventorySystem.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchForScannerPage : ContentPage
    {
        private static readonly RestService RestClient = new RestService();

        public SearchForScannerPage()
        {
            InitializeComponent();

            zxing.AutomationId = "zxingScannerView";
            overlay.AutomationId = "zxingDefaultOverlay";

            zxing.Options = new MobileBarcodeScanningOptions
            {
                DelayBetweenContinuousScans = 2000,
                AutoRotate = true,
                UseFrontCameraIfAvailable = false,
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.EAN_8, BarcodeFormat.EAN_13, BarcodeFormat.UPC_A
                }
            };

            overlay.ShowFlashButton = zxing.HasTorch;
        }

        private void Overlay_OnFlashButtonClicked(Button sender, EventArgs e)
        {
            zxing.IsTorchOn = !zxing.IsTorchOn;
        }

        private void Zxing_OnScanResult(Result result)
        {
            //Stop scanning
            zxing.IsAnalyzing = false;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.Navigation.PopAsync();

                var items = await RestClient.GetScannedItem(result.Text);

                if (items != null && items.Count > 0)
                {
                    var dictionaryOfChoices = new Dictionary<string, string>();
                    var choices = new List<string>();
                    foreach (var itemInItems in items)
                    {
                        dictionaryOfChoices.Add($"{itemInItems.Name} - {itemInItems.DateAdded}",
                            itemInItems.Id.ToString());
                        choices.Add($"{itemInItems.Name} - {itemInItems.DateAdded}");
                    }

                    var userChoice = await Shell.Current.DisplayActionSheet("Wybierz przedmiot:", "Anuluj", null,
                        choices.ToArray());

                    if (userChoice.Equals("Anuluj")) return;

                    dictionaryOfChoices.TryGetValue(userChoice, out var idOfChosenItem);

                    var item = items.Find(match => match.Id.Equals(Guid.Parse(idOfChosenItem ?? string.Empty)));

                    if (item.User.Id.Equals(Guid.Parse(StaticValues.UserId)))
                    {
                        await Shell.Current.GoToAsync($"item/modify?Id={item.Id}");

                        return;
                    }

                    await Shell.Current.DisplayAlert("Znaleziono przedmiot",
                        "Jednakże nie posiadasz uprawnień, aby go zmodyfikować",
                        "Ok");

                    return;
                }

                var notFoundAlertResult = await Shell.Current.DisplayAlert("Brak wyników",
                    $"W bazie danych nie znaleziono przemiotu o kodzie: {result.Text}",
                    "Nic nie rób",
                    "Dodaj");

                if (notFoundAlertResult) return;
                await Shell.Current.GoToAsync($"item/add?Barcode={result.Text}");
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            zxing.IsScanning = true;
        }

        protected override void OnDisappearing()
        {
            zxing.IsScanning = false;

            base.OnDisappearing();
        }
    }
}