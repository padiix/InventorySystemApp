using MvvmHelpers;
using System;
using System.Windows.Input;
using InventorySystem.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set 
            {
                _welcomeMessage = value;
                OnPropertyChanged(nameof(WelcomeMessage)); // Informuj, że była zmiana na tej właściwości
            }
        }

        public MainPageViewModel()
        {
            Title = "Strona główna";

            WelcomeMessage = "Witaj, " + PublicUserViewModel.FirstName + "!"; // Wiadomość w tej zmiennej będzie pokazana na górze ekranu.

        }
    }
}