using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private static readonly Guid _itemGuid1 = new Guid("ce91ee2a-64ee-40ae-a839-ceb7194d2850");
        private static readonly Guid _itemGuid2 = new Guid("93aed42e-8b10-4d38-a01c-fb2d8ff5b487");
        private static readonly Guid _itemGuid3 = new Guid("ac2a4217-f458-4fa6-b00f-9429a7561e2d");
        private static readonly Guid _userGuid1 = Guid.NewGuid();
        private static readonly Guid _userGuid2 = Guid.NewGuid();
        private static readonly User _user1 = new User() { Id = _userGuid1, FirstName = "John", LastName = "Doe", Email = "joed@test.com" };
        private static readonly User _user2 = new User() { Id = _userGuid2, FirstName = "Alex", LastName = "Frown", Email = "alexf@test.com" };
        private readonly Item _item1 = new Item() { Id = _itemGuid1, Barcode = "barcode_example1", Name = "Item1", DateAdded = DateTime.Today, /*UserId = "1",*/ User = _user1 };
        private readonly Item _item2 = new Item() { Id = _itemGuid2, Barcode = "barcode_example2", Name = "Item2", DateAdded = DateTime.Parse("08/18/2018 07:32:23"), /*UserId = "1",*/ User = _user1 };
        private readonly Item _item3 = new Item() { Id = _itemGuid3, Barcode = "barcode_example3", Name = "Item3", DateAdded = DateTime.Parse("05/23/2020 15:15:16"), /*UserId = "2",*/ User = _user2 };

        private readonly List<Item> _sourceItems;
        public ObservableCollection<Item> UserItems { get; set; }

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

        private readonly RestService _restClient;

        public Command RefreshCommand { get; }

        public MainPageViewModel()
        {
            _restClient = new RestService();

            RefreshCommand = new Command(async () => await GetConnection());
            Title = "Strona główna";
            SetWelcomeMessage();

            _sourceItems = new List<Item> { _item1, _item2, _item3 };
            InitCollectionViewWithFilter();
        }

        public async Task GetConnection()
        {
            if (await _restClient.GetCurrentUser())
            {
                DependencyService.Get<IMessage>().ShortAlert("Połączenie nawiązane pomyślnie.");
                MessagingCenter.Send<object>(this,"EVENT_CONNECTED_TO_API");
                return;
            }
            DependencyService.Get<IMessage>().ShortAlert("Błąd połączenia.");
        }
        private void SetWelcomeMessage()
        {
            WelcomeMessage = "Witaj, " + StaticValues.FirstName + "!"; // Wiadomość w tej zmiennej będzie pokazana na górze ekranu.
        }
        private void InitCollectionViewWithFilter()
        {
            var currentUserItems = _sourceItems.Where(item => item.User.FirstName.ToString().Contains("John")/*item => item.User.Id.ToString().Contains(StaticValues.UserId)*/).ToList();
            UserItems = new ObservableCollection<Item>(currentUserItems);

            foreach (var item in currentUserItems)
            {
                if (!currentUserItems.Contains(item))
                    UserItems.Remove(item);

                else if (!UserItems.Contains(item))
                    UserItems.Add(item);
            }
        }
    }
}