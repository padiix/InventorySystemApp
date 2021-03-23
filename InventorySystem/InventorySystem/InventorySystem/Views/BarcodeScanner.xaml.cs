
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeScanner : ZXingScannerPage
    {
        public BarcodeScanner()
        {
            InitializeComponent();
        }

        public void Handle_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Your barcode", result.Text, "Thanks");
            });
        }
    }
}