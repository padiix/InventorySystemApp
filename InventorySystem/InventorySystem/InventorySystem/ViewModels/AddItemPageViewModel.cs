using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class AddItemPageViewModel : BaseViewModel, IQueryAttributable
    {
        private Guid _itemId = new Guid();
        private string _name;
        private DateTimeOffset _dateAdded = DateTimeOffset.Now;
        private string _barcode;

        private bool _isVisibleMessageAndActivityIndicator;

        private Guid ItemId => _itemId;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }
        private DateTimeOffset DateAdded => _dateAdded;
        public string Barcode
        {
            get => _barcode;
            set => SetProperty(ref _barcode, value, nameof(Barcode));
        }
        public bool IsVisibleMessageAndActivityIndicator
        {
            get => _isVisibleMessageAndActivityIndicator;
            private set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value, nameof(IsVisibleMessageAndActivityIndicator));
        }

        private readonly RestService _restClient = new RestService();

        public Command AddItemCommand { get; }

        public AddItemPageViewModel()
        {
            AddItemCommand = new Xamarin.Forms.Command(async () =>
            {
                IsVisibleMessageAndActivityIndicator = true;

                if (await CreateItem())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.AddingItemSuccessful);
                    IsVisibleMessageAndActivityIndicator = false;
                }

                IsVisibleMessageAndActivityIndicator = false;

                await Task.Delay(TimeSpan.FromSeconds(2));

                var result = await Shell.Current.DisplayAlert("","Czy chcesz dodać więcej przedmiotów?", "Nie", "Tak");
                if (result)
                {
                    await Shell.Current.GoToAsync($"..");
                }
            });
        }

        private async Task<bool> CreateItem()
        {
            var item = new Item(ItemId, Name, Barcode, DateAdded);
            var result = await _restClient.AddItem(item);

            return result;
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            var barcode = HttpUtility.UrlDecode(query["Barcode"]);

            Barcode = string.IsNullOrEmpty(barcode) ? barcode : string.Empty;
        }
    }
}