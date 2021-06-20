using System;
using System.Text;
using InventorySystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
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
                        errorBuilder.Append(error.ToString() + "\n");

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

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return true;
        }

        private async void GoBackButton_OnClick(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}