using System;
using System.ComponentModel;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            Settings.RememberMe = false;

            LoginViewModel.Login = "";
            LoginViewModel.Password = "";

            Xamarin.Essentials.SecureStorage.Remove("token");

            MessagingCenter.Send<object>(this,App.EVENT_LAUNCH_LOGIN_PAGE);
        }
    }
}