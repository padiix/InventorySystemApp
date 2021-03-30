using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Models;

namespace InventorySystem.Interfaces
{
    public interface IRestService
    {
        Task<bool> VerifyLogin();
        //Task<PublicUserViewModel> Register();
        //Task<PublicUserViewModel> GetCurrentUser();
        Task<Item> GetItems();
        //Task<Item> GetSpecificItem(string name);
        //Task<Item> GetSpecificItem(string barcode);
        //Task<Item> UpdateItemCount();
        //Task<Item> UpdateItemInfo();
    }
}
