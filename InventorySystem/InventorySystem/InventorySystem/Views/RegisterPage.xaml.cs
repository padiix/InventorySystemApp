using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.BindingContext = new RegisterViewModel();
            Shell.SetNavBarIsVisible(this, false);
        }

        private void PasswordEntry_OnUnfocused(object sender, FocusEventArgs e)
        {
            CheckPasswordValidity();
        }

        private void CheckPasswordValidity()
        {
            if (PasswordValidationBehavior.IsValid) return;
            PasswordError.IsVisible = true;
                
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
        }

        private void PasswordEntry_OnFocused(object sender, FocusEventArgs e)
        {
            PasswordError.IsVisible = false;
        }
        
    }
}