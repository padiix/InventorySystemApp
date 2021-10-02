using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        public const string EVENT_SET_WELCOME_MESSAGE = "EVENT_SET_WELCOME_MESSAGE";
        public const string EVENT_SYNCHRONIZE_ITEMS = "EVENT_SYNCHRONIZE_ITEMS";

        //REST API Client
        private static readonly RestService
            RestClient = new RestService(); //static is used to minimalize number of Rest calls

        //RefreshView
        private bool _isCollectionViewRefreshing;

        //Fields linked to IsVisible property of connection error window.
        private bool _isErrorVisible;

        //Activity Indicator
        private bool _isVisibleMessageAndActivityIndicator;

        //Fields used to filter CollectionView with searchbar
        private string _searchValue;

        //CollectionView field and variable
        private List<Item> _sourceItems;

        //Field for Welcome Message on MainPage.xaml
        private string _welcomeMessage;


        public MainPageViewModel()
        {
            MessagingCenter.Subscribe<object>(this, EVENT_SET_WELCOME_MESSAGE,
                SetWelcomeMessage); //Called by OnAppearing of MainPage.xaml
            MessagingCenter.Subscribe<object>(this, EVENT_SYNCHRONIZE_ITEMS,
                InitCollectionView); //Called by OnAppearing of MainPage.xaml
            //

            //Inicjalizacja Komend
            RefreshCommand = new Command(async () => await GetConnection());
            //
            RefreshItemsCommand = new Command(async () => await RefreshViewGetConnection());
            //
            MoveToModificationPageCommand = new Command<Item>(async model =>
            {
                await Shell.Current.GoToAsync($"item/modify?Id={model.Id}"); //Goto with queried item inside path
            });
            //
            AddItemCommand = new Command(async () => { await Shell.Current.GoToAsync("item/add"); });
            //
            DeleteItemCommand = new Command<Item>(async model =>
            {
                var result =
                    await Shell.Current.DisplayAlert("", "Czy na pewno chcesz usunąć ten przedmiot?", "Nie", "Tak");
                if (result) return;
                DependencyService.Get<IMessage>().ShortAlert($"Usuwam przedmiot o nazwie {model.Name}");

                await RestClient.DeleteItem(model.Id);
                UserItems.Remove(
                    model); //Used for animated deletion of item and to keep up with deleted items by current user
            });
            //
            LaunchScanner = new Command(async () =>
            {
                var customScannerPage = new CustomScannerPage();
                await Shell.Current.Navigation.PushAsync(customScannerPage);
            });
        }

        public ObservableCollection<Item> UserItems { get; } = new ObservableCollection<Item>();

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }

        //All commands
        public Command RefreshCommand { get; }
        public Command RefreshItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command LaunchScanner { get; }
        public Command MoveToModificationPageCommand { get; }
        public Command DeleteItemCommand { get; }

        public bool IsCollectionViewRefreshing
        {
            get => _isCollectionViewRefreshing;
            set => SetProperty(ref _isCollectionViewRefreshing, value);
        }

        public string SearchValue
        {
            get => _searchValue;
            set => SetProperty(ref _searchValue, value);
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set => SetProperty(ref _isErrorVisible, value);
        }

        public bool IsVisibleMessageAndActivityIndicator
        {
            get => _isVisibleMessageAndActivityIndicator;
            set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value);
        }

        protected override void OnPropertyChanged(string propertyName = "")
        {
            if (propertyName.Equals(nameof(SearchValue))) FilterCollectionView();

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

            var filteredItems =
                _sourceItems.Where(item => item.Name.ToLowerInvariant().Contains(searchTerm)).ToList();
            //Search in source for items, which name has searchbar value

            foreach (var item in _sourceItems)
                if (!filteredItems.Contains(item))
                    UserItems.Remove(item);

                else if (!UserItems.Contains(item)) UserItems.Add(item);
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
            var response = await RestClient.CheckConnection();

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
            var response = await RestClient.CheckConnection();
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
            _sourceItems.Clear(); //Make sure sourceItems is empty

            try
            {
                var itemsFromApi = await RestClient.GetAllItems();

                _sourceItems.AddRange(itemsFromApi);
            }
            catch (TimeoutException toEx)
            {
                Console.WriteLine(toEx);
                IsErrorVisible = true;
                DependencyService.Get<IMessage>().ShortAlert(Constants.ConnectionError);
            }
            catch (NullReferenceException)
            {
                DependencyService.Get<IMessage>().ShortAlert(Constants.ItemsError);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            UserItems.Clear();

            foreach (var item in _sourceItems)
                if (!UserItems.Contains(item)) //If CollectionView observed list doesn't have this item, add it
                    UserItems.Add(item);

                else if (
                    !_sourceItems
                        .Contains(item)) //Else if the sourceItems list doesn't have the item, delete it from CollectionView observed list
                    UserItems.Remove(item);

            IsCollectionViewRefreshing = false; //Turn off the "refreshing" pop-up
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

        //Helpers for ActivityIndicator visibility manipulation
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