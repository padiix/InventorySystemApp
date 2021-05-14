using InventorySystem.ViewModels;
using InventorySystem.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace InventorySystem
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("scanner", typeof(BarcodeScanner));
            Routing.RegisterRoute("about", typeof(AboutApp));
            Routing.RegisterRoute("login", typeof(LoginPage));
        }
    }
}
