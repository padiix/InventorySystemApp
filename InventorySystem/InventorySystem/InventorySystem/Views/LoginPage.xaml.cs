using InventorySystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Shell.SetNavBarIsVisible(this, false);
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
            if (PasswordValidationBehavior.Errors != null)
            {
                PasswordError.IsVisible = true;
                foreach (var error in PasswordValidationBehavior.Errors)
                {
                    PasswordError.Text += error.ToString() + "\n";
                }
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