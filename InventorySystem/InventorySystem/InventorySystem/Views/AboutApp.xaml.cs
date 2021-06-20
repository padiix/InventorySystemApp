using InventorySystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutApp : ContentPage
    {
        public AboutApp()
        {
            BindingContext = new AboutAppViewModel();
            InitializeComponent();
        }
    }
}