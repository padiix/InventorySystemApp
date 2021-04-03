using System;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using MvvmHelpers;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private bool _isEmailValid;
        private bool _isPasswordValid;

        public bool IsEmailValid
        {
            get => _isEmailValid;
            set
            {
                _isEmailValid = value;
                OnPropertyChanged(nameof(IsEmailValid));
            }
        }
        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set
            {
                _isPasswordValid = value;
                OnPropertyChanged(nameof(IsPasswordValid));
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(Email);
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(Password);
            }
        }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(async () => await OnLoginClicked());
        }

        public bool IsEmailAndPasswordValid()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return !string.IsNullOrWhiteSpace(Password);
        }

        private async Task OnLoginClicked()
        {
            if (IsEmailAndPasswordValid())
            {
                var restService = new RestService();
                if (await restService.VerifyLogin(Email, Password))
                {
                    var token = Xamarin.Essentials.SecureStorage.GetAsync(RestService.Token);

                    if (token == null)
                    {
                        DependencyService.Get<IMessage>().LongAlert("Brak tokena uwierzytelniającego.");
                        return;
                    }

                    Application.Current.MainPage = new AppShell();
                    if (Settings.FirstRun)
                    {
                        Settings.FirstRun = false;
                        await Shell.Current.GoToAsync("//about");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("//main");
                    }

                }
                else
                {
                    DependencyService.Get<IMessage>().LongAlert("Weryfikacja zakończona niepowodzeniem.");
                    return;
                }
            }
            else
            {
                DependencyService.Get<IMessage>().LongAlert("Uzupełnij pola Email oraz Password!");
            }
        }
    }
}
