using InventorySystem.Views;
using System;
using InventorySystem.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            if (Settings.RememberMe == true)
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
