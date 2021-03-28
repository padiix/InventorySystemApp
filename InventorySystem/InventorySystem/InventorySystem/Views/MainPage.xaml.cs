using System;
using System.ComponentModel;
using InventorySystem.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Settings.RememberMe = false;
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            //After implementation of the API TODO: Assure to clear the username and password from data.
            //await Xamarin.Essentials.SecureStorage.SetAsync("user-name", "");
            //await Xamarin.Essentials.SecureStorage.SetAsync("password", "");
        }
    }
}