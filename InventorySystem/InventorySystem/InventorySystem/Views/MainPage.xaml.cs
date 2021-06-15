using System;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.Views
{
    public partial class MainPage : ContentPage
    {
        private static readonly RestService RestClient = new RestService();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var response = await RestClient.CheckConnection();
            switch (response)
            {
                case RestService.Connection_Connected:
                    DisconnectedMessage.IsVisible = false;
                    MessagingCenter.Send<object>(this, MainPageViewModel.EVENT_SET_WELCOME_MESSAGE);
                    MessagingCenter.Send<object>(this, MainPageViewModel.EVENT_SYNCHRONIZE_ITEMS);
                    break;

                case RestService.Connection_TokenExpired:
                    DependencyService.Get<IMessage>().LongAlert(Constants.ExpiredTokenError);

                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_NoTokenFound:
                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_ConnectionError:
                    DisconnectedMessage.IsVisible = true;
                    break;

                case RestService.Connection_StatusFailure:
                    break;
            }
        }

        private async void Logout_OnClicked(object sender, EventArgs e)
        {
            DeleteUserDetails();

            await Application.Current.SavePropertiesAsync();

            Xamarin.Essentials.SecureStorage.Remove(RestService.Token);

            MessagingCenter.Send<object>(this, App.EVENT_NAVIGATE_TO_LOGIN_PAGE);
        }

        private void Account_OnClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("account/details");
        }
        private static void DeleteUserDetails()
        {
            Settings.RememberMe = false;
            StaticValues.UserId = string.Empty;
            StaticValues.FirstName = string.Empty;
            StaticValues.LastName = string.Empty;
            StaticValues.Username = string.Empty;
            StaticValues.Email = string.Empty;
            StaticValues.IsAdmin = false;
        }
        private async void ReturnUserToLoginPage()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            Application.Current.MainPage = new EmptyAppShell();
        }
    }
}