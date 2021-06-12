using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using InventorySystem.Views;
using MvvmHelpers;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class ModifyItemPageViewModel : BaseViewModel, IQueryAttributable
    {
        private string _itemId;
        private string _name;
        private DateTimeOffset _dateAdded;
        private string _barcode;
        private bool _isVisibleMessageAndActivityIndicator;
        public string ItemId
        {
            get => _itemId;
            private set => SetProperty(ref _itemId, value, nameof(ItemId));
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }
        public DateTimeOffset DateAdded
        {
            get => _dateAdded;
            private set => SetProperty(ref _dateAdded, value);
        }
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

        public Command SaveItemCommand { get; }

        public ModifyItemPageViewModel()
        {
            SaveItemCommand = new Command(async () =>
            {
                IsVisibleMessageAndActivityIndicator = true;

                if (await UpdateItem())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.UpdateSuccessful);
                    IsVisibleMessageAndActivityIndicator = false;
                }

                IsVisibleMessageAndActivityIndicator = false;
            });
        }



        private async void LoadItem(string itemId)
        {
            try
            {
                var item = await _restClient.GetSpecificItem(itemId);

                Name = item.Name;
                DateAdded = item.DateAdded;
                Barcode = item.Barcode;
                DateAdded = item.DateAdded;
            }
            catch (Exception e)
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.ItemError);
                Console.WriteLine(new Exception($"Failed to load item ... + {e}"));
            }
        }

        private async Task<bool> UpdateItem()
        {
            var item = new Item(Guid.Parse(ItemId), Name, Barcode, DateAdded);
            var result = await _restClient.UpdateItem(Guid.Parse(ItemId), item);

            return result;
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            ItemId = HttpUtility.UrlDecode(query["Id"]);

            LoadItem(ItemId);
        }
    }
}