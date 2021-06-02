using System;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Xamarin.Forms;

namespace InventorySystem.Views
{
    public partial class MainPage : ContentPage
    {

        private readonly RestService _restClient = new RestService();

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();
        }

        protected override async void OnAppearing()
        {
            if (await _restClient.GetCurrentUser())
            {
                DisconnectedMessage.IsVisible = false;
                MessagingCenter.Send<object>(this, MainPageViewModel.EVENT_SET_WELCOME_MESSAGE);
            }
            else
            {
                DisconnectedMessage.IsVisible = true;
            }

            MessagingCenter.Send<object>(this, MainPageViewModel.EVENT_SYNCHRONIZE_ITEMS);
        }
        

        private async void Logout_OnClicked(object sender, EventArgs e)
        {
            Settings.RememberMe = false;
            StaticValues.UserId = Guid.Empty.ToString();
            StaticValues.FirstName = string.Empty;
            StaticValues.LastName = string.Empty;
            StaticValues.Username = string.Empty;
            StaticValues.Email = string.Empty;

            await Application.Current.SavePropertiesAsync();

            Xamarin.Essentials.SecureStorage.Remove(RestService.Token);

            MessagingCenter.Send<object>(this, App.EVENT_NAVIGATE_TO_LOGIN_PAGE);
        }

        private void Account_OnClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("account/details");
        }
    }
}