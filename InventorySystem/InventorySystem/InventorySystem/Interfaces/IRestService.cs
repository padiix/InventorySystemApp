using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventorySystem.Models;

namespace InventorySystem.Interfaces
{
    public interface IRestService
    {
        Task<bool> VerifyLogin(Login valuesLogin);
        Task<bool> RegisterUser(Register user);
        Task<string> CheckConnection();
        Task<List<Item>> GetAllItems();
        Task<Item> GetSpecificItem(string id);
        Task<List<Item>> GetScannedItem(string barcode);
        Task<bool> UpdateItem(Item item);
        Task<bool> DeleteItem(Guid itemId);
        Task<bool> AddItem(Item item);
    }
}