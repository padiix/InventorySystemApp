using InventorySystem.Services;
using Xamarin.Forms;

namespace InventorySystem
{
    public partial class App : Application
    {
        public const string EVENT_NAVIGATE_TO_ABOUT_PAGE = "EVENT_NAVIGATE_TO_ABOUT_PAGE";
        public const string EVENT_NAVIGATE_TO_LOGIN_PAGE = "EVENT_NAVIGATE_TO_LOGIN_PAGE";

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object>(this, EVENT_NAVIGATE_TO_LOGIN_PAGE, NavigateToLoginPage);
            MessagingCenter.Subscribe<object>(this, EVENT_NAVIGATE_TO_ABOUT_PAGE, NavigateToAboutPage);

            if (Settings.RememberMe)
                MainPage = new AppShell();
            else
                MainPage = new EmptyAppShell();
        }

        public void NavigateToLoginPage(object sender)
        {
            MainPage = new EmptyAppShell();
        }

        public async void NavigateToAboutPage(object sender)
        {
            MainPage = new AppShell();
            await Shell.Current.GoToAsync("//about");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            if (Settings.RememberMe)
                MainPage = new AppShell();
            else
                MainPage = new EmptyAppShell();
        }
    }
}