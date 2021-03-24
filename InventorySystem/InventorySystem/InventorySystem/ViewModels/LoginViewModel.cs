using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            //Need to create connectivity with API first, then we can work from there.
            //await Xamarin.Essentials.SecureStorage.SetAsync("user-name", Login);
            //Login = "";
            //await Xamarin.Essentials.SecureStorage.SetAsync("password", Password);
            //Password = "";

            Application.Current.MainPage = new AppShell();
            if (Settings.FirstRun)
            {
                Settings.FirstRun = false;
                await Shell.Current.GoToAsync($"//{nameof(AboutApp)}");
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
        }
    }
}
