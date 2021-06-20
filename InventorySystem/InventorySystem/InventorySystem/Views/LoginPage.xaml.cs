using System;
using System.Text;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void RememberMeChkbox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Settings.RememberMe = RememberMeChkbox.IsChecked;
            await Application.Current.SavePropertiesAsync();
        }

        private void CheckPasswordValidity()
        {
            if (PasswordValidationBehavior.IsValid)
            {
                PasswordError.Text = "";
                PasswordError.IsVisible = false;
                return;
            }

            var errorBuilder = new StringBuilder();

            errorBuilder.Append("Hasło musi mieć:\n");

            if (PasswordValidationBehavior.Errors != null)
                foreach (var error in PasswordValidationBehavior.Errors)
                    if (error is string)
                        errorBuilder.Append(error + "\n");

            PasswordError.Text = errorBuilder.ToString();
            PasswordError.IsVisible = true;
        }

        private void PasswordEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordEntry.Text))
            {
                PasswordError.Text = "";
                PasswordError.IsVisible = false;
                return;
            }

            CheckPasswordValidity();
        }

        private async void UserRegisterButton_OnClick(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}