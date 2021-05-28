using InventorySystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountDetailsPage : ContentPage
    {
        public AccountDetailsPage()
        {
            InitializeComponent();
            BindingContext = new AccountDetailsBindingObject();
        }
    }
}