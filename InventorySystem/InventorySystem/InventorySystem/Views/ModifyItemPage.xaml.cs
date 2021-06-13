using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventorySystem.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModifyItemPage : ContentPage
    {
        public const string EVENT_RETURN_TO_MAIN_PAGE = "EVENT_RETURN_TO_MAIN_PAGE";
        
        public ModifyItemPage()
        {
            InitializeComponent();
            MessagingCenter.Subscribe<object>(this, EVENT_RETURN_TO_MAIN_PAGE, NavigateBack);
        }

        private async void NavigateBack(object sender)
        {
            await Navigation.PopAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            MessagingCenter.Send(this, EVENT_RETURN_TO_MAIN_PAGE);
            return false;
        }

        private async void Return_OnClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            return;
        }

        private void BarcodeEntry_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var result = BarcodeValidate();
            CharacterQuantityValidator.IsValid = result;
            BarcodeNotValidLabel.IsVisible = !result;
            UpdateButton.IsEnabled = result;
        }

        private bool BarcodeValidate()
        {
            var barcode = BarcodeEntry.Text;

            if (string.IsNullOrEmpty(barcode)) return false;

            if ( barcode.Length < 8 && barcode.Length > 14 ) return false;

            barcode = barcode.PadLeft(14, '0'); // stuff zeros at start to guarantee 14 digits
            var multiplication = Enumerable.Range(0, 13).Select(i => ((int)(barcode[i] - '0')) * ((i % 2 == 0) ? 3 : 1)).ToArray(); // STEP 1: without check digit, "Multiply value of each position" by 3 or 1
            var sum = multiplication.Sum(); // STEP 2: "Add results together to create sum"
            
            if ((10 - (sum % 10)) % 10 == int.Parse(barcode[13].ToString())) // STEP 3 Equivalent to "Subtract the sum from the nearest equal or higher multiple of ten = CHECK DIGIT"
            {
                return true; 
            }

            return false;
        }
    }
}