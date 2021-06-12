using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventorySystem.Models;

namespace InventorySystem.Interfaces
{
    public interface IRestService
    {
        Task<bool> VerifyLogin(string email, string password);
        Task<bool> Register(string username, string firstname, string lastname, string email, string password);
        Task<bool> GetCurrentUser();
        Task<List<Item>> GetAllItems();
        Task<Item> GetSpecificItem(string barcode);
        Task<bool> UpdateItem(Guid itemId, Item item);
        Task<bool> DeleteItem(Guid itemId);
        Task<bool> AddItem(Item item);
    }
}
