using MvvmHelpers;
using System;
using System.Windows.Input;
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

            WelcomeMessage = "Witaj, " + "nazwa użytkownika" + "!"; // It will be shown at your label

        }
    }
}