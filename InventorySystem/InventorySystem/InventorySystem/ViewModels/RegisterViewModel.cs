using System.Collections.Generic;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using MvvmHelpers;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private static readonly RestService RestClient = new RestService();
        private string _email;
        private string _firstname;

        private bool _isActivityIndicatorRunning;

        // Booleans for setting validity of values inside Email and Password fields
        private bool _isEmailValid;

        private bool _isEnabledRegisterButton;

        private bool _isEnabledReturnButton;

        private bool _isPasswordValid;

        private bool _isReadOnlyEmail;

        // Bindings for enabling/disabling elements in ViewModel
        private bool _isReadOnlyFirstname;

        private bool _isReadOnlyLastname;

        private bool _isReadOnlyPassword;

        private bool _isReadOnlyUsername;

        //Activity indicator stuff
        private bool _isRegistrationMessageVisible;
        private string _lastname;
        private string _password;

        private string _username;

        public RegisterViewModel()
        {
            EnableElements();

            RegisterCommand = new Command(async () => await OnRegisterClick());
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Firstname
        {
            get => _firstname;
            set => SetProperty(ref _firstname, value);
        }

        public string Lastname
        {
            get => _lastname;
            set => SetProperty(ref _lastname, value);
        }

        public bool IsEmailValid
        {
            get => _isEmailValid;
            set => SetProperty(ref _isEmailValid, value);
        }

        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set => SetProperty(ref _isPasswordValid, value);
        }

        public bool IsReadOnlyFirstname
        {
            get => _isReadOnlyFirstname;
            set => SetProperty(ref _isReadOnlyFirstname, value);
        }

        public bool IsReadOnlyLastname
        {
            get => _isReadOnlyLastname;
            set => SetProperty(ref _isReadOnlyLastname, value);
        }

        public bool IsReadOnlyUsername
        {
            get => _isReadOnlyUsername;
            set => SetProperty(ref _isReadOnlyUsername, value);
        }

        public bool IsReadOnlyEmail
        {
            get => _isReadOnlyEmail;
            set => SetProperty(ref _isReadOnlyEmail, value);
        }

        public bool IsReadOnlyPassword
        {
            get => _isReadOnlyPassword;
            set => SetProperty(ref _isReadOnlyPassword, value);
        }

        public bool IsEnabledRegisterButton
        {
            get => _isEnabledRegisterButton;
            set => SetProperty(ref _isEnabledRegisterButton, value);
        }

        public bool IsEnabledReturnButton
        {
            get => _isEnabledReturnButton;
            set => SetProperty(ref _isEnabledReturnButton, value);
        }

        public bool IsRegistrationMessageVisible
        {
            get => _isRegistrationMessageVisible;
            set => SetProperty(ref _isRegistrationMessageVisible, value);
        }

        public bool IsActivityIndicatorRunning
        {
            get => _isActivityIndicatorRunning;
            set => SetProperty(ref _isActivityIndicatorRunning, value);
        }


        public Command RegisterCommand { get; }

        private bool AreFieldsNotNull()
        {
            var fields = new List<string> {Firstname, Lastname, Username, Email, Password};
            bool[] validation = {false, false, false, false, false};

            var iterator = 0;
            foreach (var field in fields)
            {
                validation[iterator] = !string.IsNullOrEmpty(field);
                iterator++;
            }

            var areFieldsNotNull = true;
            foreach (var t in validation)
                if (t == false)
                    areFieldsNotNull = false;

            return areFieldsNotNull;
        }

        private async Task OnRegisterClick()
        {
            if (!AreFieldsNotNull())
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.FillInFieldsError);
                return;
            }

            ShowActivityIndicator();
            var data = new Register
                {Email = Email, FirstName = Firstname, LastName = Lastname, Password = Password, UserName = Username};
            var response = await RestClient.RegisterUser(data);

            if (response)
            {
                await Application.Current.MainPage.DisplayAlert("Powiadomienie", "Rejestracja przebiegła pomyślnie!",
                    "OK");

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