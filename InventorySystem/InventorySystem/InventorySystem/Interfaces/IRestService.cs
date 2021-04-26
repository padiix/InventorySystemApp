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
        //Task<Item> UpdateItemCount();
        //Task<Item> UpdateItemInfo();
        Task<bool> DeleteItem(Guid itemId);
    }
}
