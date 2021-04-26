using InventorySystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Models.CustomValidators;
using InventorySystem.Services;
using Xamarin.CommunityToolkit.Behaviors;
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
            this.BindingContext = new LoginViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void RememberMeChkbox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Settings.RememberMe = (RememberMeChkbox.IsChecked == true);
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
                {
                    if (error is string)
                    {
                        errorBuilder.Append((string) error.ToString() + "\n");
                    }
                }

            PasswordError.Text = errorBuilder.ToString();
            PasswordError.IsVisible = true;
        }

        private void PasswordEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckPasswordValidity();
        }
    }
}