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
            Init();
        }

        private void Init()
        {
            MainTabBar.CurrentItem = LoginPage;
        }
    }
}
