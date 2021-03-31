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

        private void RememberMeChkbox_OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (RememberMeChkbox.IsChecked == true)
            {
                Settings.RememberMe = true;
            }
            else
            {
                Settings.RememberMe = false;
            }
                
        }

        private void CheckPasswordValidity()
        {
            if (!PasswordValidationBehavior.IsValid)
            {
                PasswordError.IsVisible = true;
                
                var errorBuilder = new StringBuilder();
                errorBuilder.Append("Password must have:\n");
                foreach (var error in PasswordValidationBehavior.Errors)
                {
                    if (error is string)
                    {
                        errorBuilder.Append((string) error.ToString() + "\n");
                    }
                }
                
                PasswordError.Text = errorBuilder.ToString();
            }
        }

        private void PasswordEntry_OnUnfocused(object sender, FocusEventArgs e)
        {
            CheckPasswordValidity();
        }

        private void PasswordEntry_OnFocused(object sender, FocusEventArgs e)
        {
            PasswordError.IsVisible = false;
            PasswordError.Text = "";
        }
    }
}