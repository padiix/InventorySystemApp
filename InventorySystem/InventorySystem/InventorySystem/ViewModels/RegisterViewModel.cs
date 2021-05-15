using System;
using System.ComponentModel;
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

        public string Username { get => _username; set => SetProperty(ref _username, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        public string Firstname { get => _firstname; set => SetProperty(ref _firstname, value); }
        public string Lastname { get => _lastname; set => SetProperty(ref _lastname, value); }

        // Booleans for setting validity of values inside Email and Password fields
        // TODO: Text only validation for FirstName & LastName
        private bool _isEmailValid;
        public bool IsEmailValid { get => _isEmailValid; set => SetProperty(ref _isEmailValid, value); }

        private bool _isPasswordValid;
        public bool IsPasswordValid { get => _isPasswordValid; set => SetProperty(ref _isPasswordValid, value); }

        // Bindings for enabling/disabling elements in ViewModel
        private bool _isReadOnlyFirstname;
        public bool IsReadOnlyFirstname { get => _isReadOnlyFirstname; set => SetProperty(ref _isReadOnlyFirstname, value); }

        private bool _isReadOnlyLastname;
        public bool IsReadOnlyLastname { get => _isReadOnlyLastname; set => SetProperty(ref _isReadOnlyLastname, value); }

        private bool _isReadOnlyUsername;
        public bool IsReadOnlyUsername { get => _isReadOnlyUsername; set => SetProperty(ref _isReadOnlyUsername, value); }

        private bool _isReadOnlyEmail;
        public bool IsReadOnlyEmail { get => _isReadOnlyEmail; set => SetProperty(ref _isReadOnlyEmail, value); }

        private bool _isReadOnlyPassword;
        public bool IsReadOnlyPassword { get => _isReadOnlyPassword; set => SetProperty(ref _isReadOnlyPassword, value); }

        private bool _isEnabledRegisterButton;
        public bool IsEnabledRegisterButton { get => _isEnabledRegisterButton; set => SetProperty(ref _isEnabledRegisterButton, value); }

        private bool _isEnabledReturnButton;
        public bool IsEnabledReturnButton { get => _isEnabledReturnButton; set => SetProperty(ref _isEnabledReturnButton, value); }

        //Activity indicator stuff
        private bool _isRegistrationMessageVisible;
        public bool IsRegistrationMessageVisible { get => _isRegistrationMessageVisible; set => SetProperty(ref _isRegistrationMessageVisible, value); }

        private bool _isActivityIndicatorRunning;
        public bool IsActivityIndicatorRunning { get => _isActivityIndicatorRunning; set => SetProperty(ref _isActivityIndicatorRunning, value); }


        public Command RegisterCommand { get; }

        public RegisterViewModel()
        {
            EnableElements();

            RegisterCommand = new Command(async () => await OnRegisterClick());
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

            ShowActivityIndicator();

            var restClient = new RestService();
            var response = await restClient.Register(Username, Firstname, Lastname, Email, Password);
            
            if (response)
            {
                await Application.Current.MainPage.DisplayAlert("Powiadomienie", "Rejestracja przebiegła pomyślnie!", "OK");

                HideActivityIndicator();
                //Automatically logs user in
                MessagingCenter.Send<object>(this, App.EVENT_NAVIGATE_TO_MAIN_PAGE);
            }
            else
            { 
                HideActivityIndicator();
                DependencyService.Get<IMessage>().LongAlert(Constants.ConnectionError);
            }

        }

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue)) return false;
            field = newValue;
            OnPropertyChanged(propertyName ?? string.Empty);
            return true;
        }

        private void EnableElements()
        {
            IsReadOnlyEmail = false;
            IsReadOnlyFirstname = false;
            IsReadOnlyLastname = false;
            IsReadOnlyUsername = false;
            IsReadOnlyPassword = false;
            
            IsEnabledRegisterButton = true;
            IsEnabledReturnButton = true;
        }

        private void DisableElements()
        {
            IsReadOnlyEmail = true;
            IsReadOnlyFirstname = true;
            IsReadOnlyLastname = true;
            IsReadOnlyUsername = true;
            IsReadOnlyPassword = true;
            
            IsEnabledRegisterButton = false;
            IsEnabledReturnButton = false;
        }

        private void ShowActivityIndicator()
        {
            DisableElements();

            IsRegistrationMessageVisible = true;
            IsActivityIndicatorRunning = true;
        }

        private void HideActivityIndicator()
        {
            EnableElements();

            IsRegistrationMessageVisible = false;
            IsActivityIndicatorRunning = false;
        }
    }
}