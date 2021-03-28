using InventorySystem.Interfaces;
using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Models;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public static string Login;
        public static string Password;

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private async void OnLoginClicked(object obj)
        {
            if (Login == null || Password == null)
            {
                DependencyService.Get<IMessage>().ShortAlert("Uzupełnij pole \"login\" lub \"hasło\"");
                return;
            }

            Application.Current.MainPage = new AppShell();

            if (Settings.FirstRun)
            {
                Settings.FirstRun = false;
                await Shell.Current.GoToAsync($"//about");
            }
            else
            {
                await Shell.Current.GoToAsync("//main");
            }
        }
    }
}
