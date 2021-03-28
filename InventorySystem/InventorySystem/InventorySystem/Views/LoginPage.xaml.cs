using InventorySystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Services;
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

        private void Login_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LoginViewModel.Login = e.NewTextValue;
        }

        private void Password_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LoginViewModel.Password = e.NewTextValue;
        }
    }
}