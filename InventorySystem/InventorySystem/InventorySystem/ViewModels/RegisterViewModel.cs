using System;
using System.Threading.Tasks;
using InventorySystem.Models;
using MvvmHelpers;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using InventorySystem.Interfaces;
using InventorySystem.Services;

namespace InventorySystem.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _username;
        private string _email;
        private string _password;
        private string _firstname;
        private string _lastname;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public string Firstname
        {
            get => _firstname;
            set
            {
                _firstname = value;
                OnPropertyChanged(nameof(Firstname));
            }
        }
        public string Lastname
        {
            get => _lastname;
            set
            {
                _lastname = value;
                OnPropertyChanged(nameof(Lastname));
            }
        }


        private bool _isEmailValid;
        public bool IsEmailValid
        {
            get => _isEmailValid;
            set
            {
                _isEmailValid = value;
                OnPropertyChanged(nameof(IsEmailValid));
            }
        }

        private bool _isPasswordValid;
        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set
            {
                _isPasswordValid = value;
                OnPropertyChanged(nameof(IsPasswordValid));
            }
        }

        public Command RegisterCommand { get; }
        public Command ReturnCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = new Command(async () => await OnRegisterClick());
            ReturnCommand = new Command(async () => await OnReturnClick());
        }

        private bool AreFieldsNotNull()
        {
            if (string.IsNullOrWhiteSpace(Firstname)) return false;
            if (string.IsNullOrWhiteSpace(Lastname)) return false;
            if (string.IsNullOrWhiteSpace(Username)) return false;
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return !string.IsNullOrWhiteSpace(Password);
        }

        private async Task OnRegisterClick()
        {
            if (!AreFieldsNotNull())
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.FillInFieldsError);
                return;
            }

            var restClient = new RestService();
            if (await restClient.Register(Username, Firstname, Lastname, Email, Password))
            {
                await Application.Current.MainPage.DisplayAlert("Powiadomienie", "Rejestracja przebiegła pomyślnie!", "OK");

                //Automatically logs user in
                MessagingCenter.Send<object>(this, App.EVENT_NAVIGATE_TO_MAIN_PAGE);
            }
            else
            { 
                DependencyService.Get<IMessage>().LongAlert(Constants.ConnectionError);
            }

        }

        private async Task OnReturnClick()
        {
            await Shell.Current.GoToAsync("//login");
        }

    }
}