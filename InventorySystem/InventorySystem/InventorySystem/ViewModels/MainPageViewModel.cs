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
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public const string EVENT_SET_WELCOME_MESSAGE = "EVENT_SET_WELCOME_MESSAGE";

        //Zmienne potrzebne dla poprawnego wyświetlania danych w CollectionView
        private List<Item> _sourceItems;
        private List<Item> _currentUserItems;
        public ObservableCollection<Item> UserItems { get; } = new ObservableCollection<Item>();

        //Zmienne potrzebne do ustawiania wiadomości powitalnej
        private string _welcomeMessage;
        public string WelcomeMessage { get => _welcomeMessage; set => SetProperty(ref _welcomeMessage, value); }

        //Klient pozwalający na połączenie z API
        private readonly RestService _restClient;

        public Command RefreshCommand { get; }

        public Command AddItemCommand { get; }
        public Command MoveToModificationPageCommand { get; }
        public Command DeleteItemCommand { get; }

        //Zmienne do filtrowania CollectionView po nazwie
        private string _searchValue;
        public string SearchValue { get => _searchValue; set => SetProperty(ref _searchValue, value); }

        //Pole połączone z właściwością IsVisible dla okienka z wiadomością o połączeniu.
        public bool IsErrorVisible { get; set; } = false;

        //Activity Indicator
        private bool _isVisibleMessageAndActivityIndicator = false;

        public bool IsVisibleMessageAndActivityIndicator { get => _isVisibleMessageAndActivityIndicator; set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value); }


        public MainPageViewModel()
        {
            MessagingCenter.Subscribe<object>(this, EVENT_SET_WELCOME_MESSAGE, SetWelcomeMessage);

            _restClient = new RestService();

            RefreshCommand = new Command(async () => await GetConnection());

            //Test działania Bindingu
            MoveToModificationPageCommand = new Command<Item>(model =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Przenoszę do strony modyfikacji przemiotu o nazwie {model.Name}");
            });

            AddItemCommand = new Command(() =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Dodaję przedmiot");
            });

            DeleteItemCommand = new Command<Item>(model =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Usuwam przedmiot o nazwie {model.Name}");
            });

            InitCollectionView();
            //_sourceItems = new List<Item> { _item1, _item2, _item3 };
        }

        //private void ModifyItem(string guid = "no guid")
        //{
            
        //}

        protected override void OnPropertyChanged(string propertyName = "")
        {
            if (propertyName.Equals(nameof(SearchValue)))
            {
                FilterCollectionView();
            }

            base.OnPropertyChanged(propertyName);
        }

        private void FilterCollectionView()
        {
            var searchTerm = SearchValue;

            if (string.IsNullOrWhiteSpace(searchTerm))
                searchTerm = string.Empty;

            searchTerm = searchTerm.ToLowerInvariant();

            var filteredItems = _currentUserItems.Where(item => item.Name.ToLowerInvariant().Contains(searchTerm)).ToList();

            foreach (var item in _currentUserItems)
            {
                if (!filteredItems.Contains(item))
                    UserItems.Remove(item);

                else if (!UserItems.Contains(item))
                    UserItems.Add(item);
            }
        }

        public async Task GetConnection()
        {
            ShowActivityIndicatorWithMessage();
            var response = await _restClient.GetCurrentUser();
            if (response)
            {
                DependencyService.Get<IMessage>().ShortAlert("Połączenie nawiązane pomyślnie.");
                IsErrorVisible = false;
                await GetItemsForUser();
                HideActivityIndicatorWithMessage();
                return;
            }

            IsErrorVisible = true;
            HideActivityIndicatorWithMessage();
            DependencyService.Get<IMessage>().ShortAlert("Błąd połączenia.");
        }
        private void SetWelcomeMessage(object sender)
        {
            //Wiadomość w tej zmiennej będzie pokazana na górze ekranu na stronie głównej.
            WelcomeMessage = "Witaj, " + StaticValues.FirstName + "!";
        }

        private async Task GetItemsForUser()
        {
            _sourceItems.Clear();

            try
            {
                var itemsFromApi = await _restClient.GetAllItems();

                foreach (var item in itemsFromApi)
                {
                    _sourceItems.Add(item);
                }

                //Proponowane sprawdzanie czy elementy się powtarzają
                //foreach (var item in itemsFromAPI)
                //{
                //    if (_sourceItems.Contains(item)) continue;
                //    _sourceItems.Add(item);
                //}

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            _currentUserItems = _sourceItems;
            //    .Where(item => item.User.Id.ToString().Contains(StaticValues.UserId.ToString())).ToList();

            foreach (var item in _currentUserItems)
            {
                if (!UserItems.Contains(item))
                    UserItems.Add(item);

                else if (!_currentUserItems.Contains(item))
                    UserItems.Remove(item);
            }
        }

        private async void InitCollectionView()
        {
           _currentUserItems = new List<Item>();
            _sourceItems = new List<Item>();

            await GetItemsForUser();
        }

        private void ShowActivityIndicatorWithMessage()
        {
            IsVisibleMessageAndActivityIndicator = true;
        }

        private void HideActivityIndicatorWithMessage()
        {
            IsVisibleMessageAndActivityIndicator = false;
        }

        private bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, newValue)) return false;
            field = newValue;
            OnPropertyChanged(propertyName ?? string.Empty);
            return true;
        }
    }
}