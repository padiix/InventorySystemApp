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
        public string Login { get; private set; }
        public string Password { get; private set; }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
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
