using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Services;
using MvvmHelpers;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

namespace InventorySystem.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private static readonly RestService RestClient = new RestService();

        //Values kept in ViewModel
        private string _email;
        private string _password;
        private bool _isEmailValid;

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
        
        //Activity indicator
        private bool _connectingMessageVisibility;
        private bool _runActivityIndicator;

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

        //Objects enabling/disabling elements on LoginPage.xaml

        private bool _isEnabledRememberMe;

        public bool IsEnabledRememberMe { get => _isEnabledRememberMe; set => SetProperty(ref _isEnabledRememberMe, value); }

        private bool _isEnabledLoginButton;

        public bool IsEnabledLoginButton { get => _isEnabledLoginButton; set => SetProperty(ref _isEnabledLoginButton, value); }

        private bool _isEnabledRegisterButton;

        public bool IsEnabledRegisterButton { get => _isEnabledRegisterButton; set => SetProperty(ref _isEnabledRegisterButton, value); }

        private bool _isReadOnlyPasswordEntry;

        public bool IsReadOnlyPasswordEntry { get => _isReadOnlyPasswordEntry; set => SetProperty(ref _isReadOnlyPasswordEntry, value); }

        private bool _isReadOnlyEmailEntry;

        public bool IsReadOnlyEmailEntry { get => _isReadOnlyEmailEntry; set => SetProperty(ref _isReadOnlyEmailEntry, value); }


        //Commands
        public Command LoginCommand { get; }


        public LoginViewModel()
        {
            EnableElements();

            LoginCommand = new Command(async () => await OnLoginClicked());
        }

        public bool IsEmailAndPasswordNotNull()
        {
            if (string.IsNullOrWhiteSpace(Email)) return false;
            return !string.IsNullOrWhiteSpace(Password);
        }

        private void EnableElements()
        {
            //TODO: Figure out how to ENABLE/DISABLE elements in a runtime
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
                var isVerified = await RestClient.VerifyLogin(Email, Password);
                if (isVerified)
                {
                    HideActivityIndicator();

                    var token = Xamarin.Essentials.SecureStorage.GetAsync(RestService.Token);

                    if (token == null)
                    {
                        DependencyService.Get<IMessage>().LongAlert(Constants.NoTokenError);
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
                        await Shell.Current.GoToAsync($"//main");
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
