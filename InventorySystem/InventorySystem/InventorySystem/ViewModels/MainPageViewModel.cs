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
            MessagingCenter.Subscribe<object>(this, EVENT_SET_WELCOME_MESSAGE, SetWelcomeMessage);
            //TODO: Test if this event works
            MessagingCenter.Subscribe<object>(this, EVENT_SYNCHRONIZE_ITEMS, InitCollectionView);

            _restClient = new RestService();

            RefreshCommand = new Command(async () => await GetConnection());

            RefreshItemsCommand = new Command(async () => await GetItemsForUser());

            //Test działania Bindingu
            MoveToModificationPageCommand = new Command<Item>(model =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Przenoszę do strony modyfikacji przemiotu o nazwie {model.Name}");
                Shell.Current.GoToAsync($"item/modify?Id={model.Id.ToString()}");
            });

            AddItemCommand = new Command(/*async*/ () =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Dodaję przedmiot");
                //await Shell.Current.GoToAsync($"item/add");
            });

            DeleteItemCommand = new Command<Item>(async model =>
            {
                DependencyService.Get<IMessage>().ShortAlert($"Usuwam przedmiot o nazwie {model.Name}");
                //await _restClient.DeleteItem(model.Id);
            });
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

        private async void FilterCollectionView()
        {
            var searchTerm = SearchValue;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = string.Empty;
                await GetItemsForUser();
            }
                

            searchTerm = searchTerm.ToLowerInvariant();

            var filteredItems = _sourceItems.Where(item => item.Name.ToLowerInvariant().Contains(searchTerm)).ToList();

            foreach (var item in _sourceItems)
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
                //DependencyService.Get<IMessage>().ShortAlert("Połączenie nawiązane pomyślnie.");
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
            catch (TimeoutException toEx)
            {
                Console.WriteLine(toEx);
                IsErrorVisible = true;
                DependencyService.Get<IMessage>().ShortAlert("Błąd połączenia.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            UserItems.Clear();

            foreach (var item in _sourceItems)
            {
                if (!UserItems.Contains(item))
                    UserItems.Add(item);

                else if (!_sourceItems.Contains(item))
                    UserItems.Remove(item);
            }

            IsCollectionViewRefreshing = false;
        }

        private async void InitCollectionView(object sender)
        {
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