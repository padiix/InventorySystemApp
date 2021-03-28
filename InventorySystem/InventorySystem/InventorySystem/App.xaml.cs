using InventorySystem.Views;
using System;
using InventorySystem.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem
{
    public partial class App : Application
    {
        public const string EVENT_LAUNCH_MAIN_PAGE = "EVENT_LAUNCH_MAIN_PAGE";
        public const string EVENT_LAUNCH_LOGIN_PAGE = "EVENT_LAUNCH_LOGIN_PAGE";

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object>(this, EVENT_LAUNCH_LOGIN_PAGE, SetRootToLoginPage);
            MessagingCenter.Subscribe<object>(this, EVENT_LAUNCH_MAIN_PAGE, SetRootToMainPage);

            if (Settings.RememberMe == true)
            {
                
                MainPage = new AppShell();
            }
            else
            {
                
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        public async void SetRootToLoginPage(object sender)
        {
            await Shell.Current.GoToAsync("//login");
        }
        
        public async void SetRootToMainPage(object sender)
        {
            await Shell.Current.GoToAsync("//main");
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
