using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        private static readonly RestService RestClient = new RestService();
        private string _barcode;
        private DateTimeOffset _dateAdded;
        private bool _isVisibleMessageAndActivityIndicator;
        private string _itemId;
        private string _name;
        private string _description;

        public ModifyItemPageViewModel()
        {
            UpdateItemCommand = new Command(async () =>
            {
                IsVisibleMessageAndActivityIndicator = true;

                if (await UpdateItem())
                {
                    DependencyService.Get<IMessage>().ShortAlert(Constants.UpdateSuccessful);
                    IsVisibleMessageAndActivityIndicator = false;
                }

                IsVisibleMessageAndActivityIndicator = false;

                await Task.Delay(TimeSpan.FromSeconds(0.5));

                var result = await Shell.Current.DisplayAlert("", "Czy chcesz wrócić do strony głównej?", "Nie", "Tak");
                if (!result) await Shell.Current.GoToAsync("..");
            });
        }

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

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, nameof(Description));
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
            private set => SetProperty(ref _isVisibleMessageAndActivityIndicator, value,
                nameof(IsVisibleMessageAndActivityIndicator));
        }

        public Command UpdateItemCommand { get; }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            ItemId = HttpUtility.UrlDecode(query["Id"]);

            LoadItem(ItemId);
        }

        private async void LoadItem(string itemId)
        {
            try
            {
                var item = await RestClient.GetSpecificItem(itemId);

                Name = item.Name;
                Description = item.Description;
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
            var item = new Item(Guid.Parse(ItemId), Name, Description, Barcode, DateAdded);
            var result = await RestClient.UpdateItem(item);

            return result;
        }
    }
}