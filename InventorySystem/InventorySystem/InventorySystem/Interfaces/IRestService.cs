using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Models;

namespace InventorySystem.Interfaces
{
    public interface IRestService
    {
        Task<bool> VerifyLogin(string email, string password);
        //Task<PublicUserViewModel> Register();
        //Task<PublicUserViewModel> GetCurrentUser();
        Task<List<Item>> GetItems();
        //Task<Item> GetSpecificItem(string barcode);
        //Task<Item> UpdateItemCount();
        //Task<Item> UpdateItemInfo();
    }
}
