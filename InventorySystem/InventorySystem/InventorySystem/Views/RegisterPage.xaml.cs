using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private void PasswordEntry_OnUnfocused(object sender, FocusEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PasswordEntry_OnFocused(object sender, FocusEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send(this, "EVENT_LAUNCH_MAIN_PAGE");
            return true;
        }
    }
}