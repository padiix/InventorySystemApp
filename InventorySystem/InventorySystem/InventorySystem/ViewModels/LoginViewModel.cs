using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private static readonly RestService RestClient = new RestService();

        //Activity indicator
        private bool _connectingMessageVisibility;

        //Values kept in ViewModel
        private string _email;
        private bool _isEmailValid;

        private bool _isEnabledLoginButton;

        private bool _isEnabledRegisterButton;

        //Objects enabling/disabling elements on LoginPage.xaml

        private bool _isEnabledRememberMe;

        private bool _isReadOnlyEmailEntry;

        private bool _isReadOnlyPasswordEntry;
        private string _password;
        private bool _runActivityIndicator;


        public LoginViewModel()
        {
            EnableElements();

            LoginCommand = new Command(async () => await OnLoginClicked());
        }

        //Binded objects
        public bool IsEmailValid
        {
            get => _isEmailValid;
            set => SetProperty(ref _isEmailValid, value, nameof(IsEmailValid));
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, nameof(Email));
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, nameof(Password));
        }

        public bool ConnectingMessageVisibility
        {
            get => _connectingMessageVisibility;
            set => SetProperty(ref _connectingMessageVisibility, value, nameof(ConnectingMessageVisibility));
        }

        public bool RunActivityIndicator
        {
            get => _runActivityIndicator;
            set => SetProperty(ref _runActivityIndicator, value, nameof(RunActivityIndicator));
        }

        public bool IsEnabledRememberMe
        {
            get => _isEnabledRememberMe;
            set => SetProperty(ref _isEnabledRememberMe, value);
        }

        public bool IsEnabledLoginButton
        {
            get => _isEnabledLoginButton;
            set => SetProperty(ref _isEnabledLoginButton, value);
        }

        public bool IsEnabledRegisterButton
        {
            get => _isEnabledRegisterButton;
            set => SetProperty(ref _isEnabledRegisterButton, value);
        }

        public bool IsReadOnlyPasswordEntry
        {
            get => _isReadOnlyPasswordEntry;
            set => SetProperty(ref _isReadOnlyPasswordEntry, value);
        }

        public bool IsReadOnlyEmailEntry
        {
            get => _isReadOnlyEmailEntry;
            set => SetProperty(ref _isReadOnlyEmailEntry, value);
        }


        //Commands
        public Command LoginCommand { get; }

        public bool IsEmailAndPasswordNotNull()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return !string.IsNullOrWhiteSpace(Password);
        }

        private void EnableElements()
        {
            IsEnabledLoginButton = true;
            IsEnabledRegisterButton = true;
            IsEnabledRememberMe = true;

            IsReadOnlyEmailEntry = false;
            IsReadOnlyPasswordEntry = false;
        }

        private void DisableElements()
        {
            IsEnabledLoginButton = false;
            IsEnabledRegisterButton = false;
            IsEnabledRememberMe = false;

            IsReadOnlyEmailEntry = true;
            IsReadOnlyPasswordEntry = true;
        }

        private void ShowActivityIndicator()
        {
            DisableElements();

            ConnectingMessageVisibility = true;
            RunActivityIndicator = true;
        }

        private void HideActivityIndicator()
        {
            EnableElements();

            ConnectingMessageVisibility = false;
            RunActivityIndicator = false;
        }

        private async Task OnLoginClicked()
        {
            if (IsEmailAndPasswordNotNull())
            {
                ShowActivityIndicator();

                var data = new Login {Email = Email, Password = Password};
                var isVerified = await RestClient.VerifyLogin(data);
                if (isVerified)
                {
                    HideActivityIndicator();

                    var token = await SecureStorage.GetAsync(RestService.Token);

                    if (token == null)
                    {
                        DependencyService.Get<IMessage>().LongAlert(Constants.NoTokenError);
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

                HideActivityIndicator();
            }
            else
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.EmailAndPasswordFillInError);
            }
        }
    }
}