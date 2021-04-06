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
        private const string EVENT_CONNECTED_TO_API = "EVENT_CONNECTED_TO_API";

        private readonly RestService _restClient = new RestService();

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainPageViewModel();

            MessagingCenter.Subscribe<object>(this, EVENT_CONNECTED_TO_API, HideDisconnectedMessage);
        }

        private void HideDisconnectedMessage(object sender)
        {
            DisconnectedMessage.IsVisible = false;
        }

        protected override async void OnAppearing()
        {
            if (await _restClient.GetCurrentUser())
            {
                DisconnectedMessage.IsVisible = false;
            }
            else
            {
                DisconnectedMessage.IsVisible = true;
            }
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

            MessagingCenter.Send<object>(this, App.EVENT_LAUNCH_LOGIN_PAGE);
        }

        private void Account_OnClicked(object sender, EventArgs e)
        {
            DependencyService.Get<IMessage>().LongAlert("Pokazywanie strony z info o koncie...\n" +
                                                         $"UserId: {StaticValues.UserId}{Environment.NewLine}" +
                                                         $"FirstName: {StaticValues.FirstName}{Environment.NewLine}" +
                                                         $"LastName: {StaticValues.LastName}{Environment.NewLine}" +
                                                         $"Username: {StaticValues.Username}{Environment.NewLine}" +
                                                         $"E-mail: {StaticValues.Email}");
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}