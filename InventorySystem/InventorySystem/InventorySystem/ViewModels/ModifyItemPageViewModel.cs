using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using InventorySystem.Services;
using MvvmHelpers;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class ModifyItemPageViewModel : BaseViewModel, IQueryAttributable
    {
        private Item item;

        private string _itemId;
        private string _name;
        private DateTimeOffset _dateAdded;
        private string _barcode;


        public ModifyItemPageViewModel()
        {
            SaveItemCommand = new Command(async () =>
            {
                DependencyService.Get<IMessage>().LongAlert("Zapisywanie informacji o przedmiocie ...");
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            ItemId = HttpUtility.UrlDecode(query["Id"]);

            LoadItem(ItemId);
        }

        public string ItemId
        {
            get => _itemId;
            private set => SetProperty(ref _itemId, value);
        }

        public string Name
        {
            get => _name;
            private set => SetProperty(ref _name, value);
        }
        public DateTimeOffset DateAdded
        {
            get => _dateAdded;
            private set => SetProperty(ref _dateAdded, value);
        }
        public string Barcode
        {
            get => _barcode;
            private set => SetProperty(ref _barcode, value);
        }

        private readonly RestService _restClient = new RestService();

        public Command SaveItemCommand { get; }

        private async void LoadItem(string itemId)
        {
            try
            {
                item = await _restClient.GetSpecificItem(itemId);

                Name = item.Name;
                DateAdded = item.DateAdded;
                Barcode = item.Barcode;
            }
            catch (Exception e)
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.ItemError);
                Console.WriteLine($"Failed to load item ... + {e}");
                return;
            }
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