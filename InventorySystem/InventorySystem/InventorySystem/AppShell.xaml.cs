using InventorySystem.Views;
using Xamarin.Forms;

namespace InventorySystem
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("account/details", typeof(AccountDetailsPage));
            Routing.RegisterRoute("item/modify", typeof(ModifyItemPage));
            Routing.RegisterRoute("item/add", typeof(AddItemPage));
        }
    }
}