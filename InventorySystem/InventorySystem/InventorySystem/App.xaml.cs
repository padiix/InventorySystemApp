using InventorySystem.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            var isLogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLogged == "1")
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }


        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
