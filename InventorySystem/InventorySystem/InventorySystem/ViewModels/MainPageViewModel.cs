using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public const string EVENT_SET_WELCOME_MESSAGE = "EVENT_SET_WELCOME_MESSAGE";
        public const string EVENT_SYNCHRONIZE_ITEMS = "EVENT_SYNCHRONIZE_ITEMS";

        //Zmienne potrzebne dla poprawnego wyświetlania danych w CollectionView
        private List<Item> _sourceItems;
        public ObservableCollection<Item> UserItems { get; } = new ObservableCollection<Item>();

        //Zmienne potrzebne do ustawiania wiadomości powitalnej
        private string _welcomeMessage;
        public string WelcomeMessage { get => _welcomeMessage; set => SetProperty(ref _welcomeMessage, value); }

        //Klient pozwalający na połączenie z API
        private readonly RestService _restClient;

        //Komendy
        public Command RefreshCommand { get; }
        public Command RefreshItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command MoveToModificationPageCommand { get; }
        public Command DeleteItemCommand { get; }

        //RefreshView
        private bool _isCollectionViewRefreshing = false;
        public bool IsCollectionViewRefreshing { get => _isCollectionViewRefreshing; set => SetProperty(ref _isCollectionViewRefreshing, value); }

        //Filtrowanie CollectionView po nazwie przedmiotu
        private string _searchValue;
        public string SearchValue { get => _searchValue; set => SetProperty(ref _searchValue, value); }

        //Pole połączone z właściwością IsVisible dla okienka z wiadomością o połączeniu.
        private bool _isErrorVisible = false;
        public bool IsErrorVisible { get => _isErrorVisible; set => SetProperty(ref _isErrorVisible, value); }

        //Activity Indicator
        private bool _isVisibleMessageAndActivityIndicator = false;

        public bool IsVisibleMessageAndActivityIndicator { get => _isVisibleMessageAndActivityIndicator; set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value); }


        public MainPageViewModel()
        {
            //Uruchomienie nasłuchu na wywołania o danych nazwach.
            MessagingCenter.Subscribe<object>(this, EVENT_SET_WELCOME_MESSAGE, SetWelcomeMessage);
            MessagingCenter.Subscribe<object>(this, EVENT_SYNCHRONIZE_ITEMS, InitCollectionView);
            //

            _restClient = new RestService();


            //Inicjalizacja Komend
            RefreshCommand = new Command(async () => await GetConnection());
            //
            RefreshItemsCommand = new Command(async () => await RefreshViewGetConnection());
            //
            MoveToModificationPageCommand = new Command<Item>(async model =>
            {
                await Shell.Current.GoToAsync($"item/modify?Id={model.Id.ToString()}"); //Asynchroniczne przejście na stronę modyfikowania przedmiotów z dołączonym przedmiotem w ścieżce
            });
            //
            AddItemCommand = new Command(async () =>
            {
                await Shell.Current.GoToAsync($"item/add"); //Asynchroniczne przejście na stronę dodawania przedmiotów
            });
            //
            DeleteItemCommand = new Command<Item>(async model =>
            {
                var result = await Shell.Current.DisplayAlert("","Czy na pewno chcesz usunąć ten przedmiot?", "Nie", "Tak");
                if (result) return;
                DependencyService.Get<IMessage>().ShortAlert($"Usuwam przedmiot o nazwie {model.Name}");

                await _restClient.DeleteItem(model.Id);
                UserItems.Remove(model);
            });
            //
        }

        protected override void OnPropertyChanged(string propertyName = "")
        {
            if (propertyName.Equals(nameof(SearchValue)))
            {
                FilterCollectionView();
            }

            base.OnPropertyChanged(propertyName);
        }

        private async void FilterCollectionView()
        {
            var searchTerm = SearchValue; //Weź wartość wpisaną w wyszukiwarce

            if (string.IsNullOrWhiteSpace(searchTerm)) //Sprawdź czy nie jest pusta
            {
                searchTerm = string.Empty;
                await GetItemsForUser();
            }
                

            searchTerm = searchTerm.ToLowerInvariant();

            var filteredItems = _sourceItems.Where(item => item.Name.ToLowerInvariant().Contains(searchTerm)).ToList(); //Wyszukaj w liście zródłowej przedmioty, których nazwa zawiera wyszukiwaną wartość

            foreach (var item in _sourceItems)
            {
                if (!filteredItems.Contains(item))
                {
                    UserItems.Remove(item);
                }

                else if (!UserItems.Contains(item))
                {
                    UserItems.Add(item);
                }
            }
        }

        private static void DeleteUserDetails()
        {
            StaticValues.UserId = string.Empty;
            StaticValues.FirstName = string.Empty;
            StaticValues.LastName = string.Empty;
            StaticValues.Username = string.Empty;
            StaticValues.Email = string.Empty;
            StaticValues.IsAdmin = false;
        }

        public async Task GetConnection()
        {
            ShowActivityIndicatorWithMessage();
            var response = await _restClient.CheckConnection();

            switch (response)
            {
                case RestService.Connection_Connected:
                    IsErrorVisible = false;
                    await GetItemsForUser();
                    HideActivityIndicatorWithMessage();
                    break;

                case RestService.Connection_TokenExpired:
                    DependencyService.Get<IMessage>().LongAlert(Constants.ExpiredTokenError);
                    HideActivityIndicatorWithMessage();
                    
                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_NoTokenFound:
                    HideActivityIndicatorWithMessage();
                    
                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_ConnectionError:
                    IsErrorVisible = true;
                    HideActivityIndicatorWithMessage();
                    break;

                case RestService.Connection_StatusFailure:
                    HideActivityIndicatorWithMessage();
                    break;
            }
        }
        public async Task RefreshViewGetConnection()
        {
            var response = await _restClient.CheckConnection();
            switch (response)
            {
                case RestService.Connection_Connected:
                    IsErrorVisible = false;
                    await GetItemsForUser();
                    break;

                case RestService.Connection_TokenExpired:
                    DependencyService.Get<IMessage>().LongAlert(Constants.ExpiredTokenError);

                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_NoTokenFound:
                    SecureStorage.Remove(RestService.Token);
                    DeleteUserDetails();
                    await Application.Current.SavePropertiesAsync();

                    ReturnUserToLoginPage();
                    break;

                case RestService.Connection_ConnectionError:
                    IsErrorVisible = true;
                    break;

                case RestService.Connection_StatusFailure:
                    break;
            }
        }
        private void SetWelcomeMessage(object sender)
        {
            //Wiadomość w tej zmiennej będzie pokazana na górze ekranu na stronie głównej.
            WelcomeMessage = "Witaj, " + StaticValues.FirstName + "!";
        }

        private async Task GetItemsForUser()
        {
            _sourceItems.Clear(); //Upewnij się że lista przedmiotów jest czysta

            try
            {
                var itemsFromApi = await _restClient.GetAllItems(); //Próbuuj ściągnąć przedmioty z API

                _sourceItems.AddRange(itemsFromApi); //Wrzuć je do listy
            }
            catch (TimeoutException toEx)
            {
                Console.WriteLine(toEx);
                IsErrorVisible = true;
                DependencyService.Get<IMessage>().ShortAlert(Constants.ConnectionError);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            UserItems.Clear(); 

            foreach (var item in _sourceItems)
            {
                if (!UserItems.Contains(item)) //Jeżeli obserwowana lista nie ma danego przedmiotu, dodaj go
                    UserItems.Add(item);

                else if (!_sourceItems.Contains(item)) // W przeciwnym wypadku, jeżeli lista przedmiotów nie posiada elementu z obserwowanej listy, usuń go
                    UserItems.Remove(item);
            }

            IsCollectionViewRefreshing = false; //Daj znać, że Collection View skończył się odświeżać
        }

        private async void InitCollectionView(object sender)
        {
            _sourceItems = new List<Item>();
            await GetItemsForUser();
        }

        private async void ReturnUserToLoginPage()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5));

            Application.Current.MainPage = new EmptyAppShell();
        }

        //Metody kontrolujące widzialność indykatora aktywności
        private void ShowActivityIndicatorWithMessage()
        {
            IsVisibleMessageAndActivityIndicator = true;
        }
        //
        private void HideActivityIndicatorWithMessage()
        {
            IsVisibleMessageAndActivityIndicator = false;
        }
        //
    }
}