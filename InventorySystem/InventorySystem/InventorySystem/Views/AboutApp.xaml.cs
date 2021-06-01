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
    public partial class AboutApp : ContentPage
    {
        public AboutApp()
        {
            this.BindingContext = new AboutAppViewModel();
            InitializeComponent();
        }
    }
}