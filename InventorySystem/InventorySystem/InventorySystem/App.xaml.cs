using InventorySystem.Services;
using Xamarin.Forms;

namespace InventorySystem
{
    public partial class App : Application
    {
        public const string EVENT_NAVIGATE_TO_MAIN_PAGE = "EVENT_NAVIGATE_TO_MAIN_PAGE";
        public const string EVENT_NAVIGATE_TO_LOGIN_PAGE = "EVENT_NAVIGATE_TO_LOGIN_PAGE";

        public App()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<object>(this, EVENT_NAVIGATE_TO_LOGIN_PAGE, NavigateToLoginPage);
            MessagingCenter.Subscribe<object>(this, EVENT_NAVIGATE_TO_MAIN_PAGE, NavigateToMainPage);

            if (Settings.RememberMe == true)
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new EmptyAppShell();
            }
        }

        public void NavigateToLoginPage(object sender)
        {
            MainPage = new EmptyAppShell();
        }
        
        public async void NavigateToMainPage(object sender)
        {
            MainPage = new AppShell();
            await Shell.Current.GoToAsync($"//main");
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
