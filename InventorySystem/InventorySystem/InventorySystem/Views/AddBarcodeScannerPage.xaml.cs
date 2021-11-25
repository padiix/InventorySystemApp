using System;
using System.Collections.Generic;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddBarcodeScannerPage : ContentPage
    {
        public AddBarcodeScannerPage()
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

            Preferences.Set("ScannedBarcode", result.Text);

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
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