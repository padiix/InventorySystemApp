using InventorySystem.Models;
using Xamarin.Forms;

namespace InventorySystem.ViewModels
{
    public class AccountDetailsBindingObject : BindableObject
    {
        public string Id => StaticValues.UserId;
        public string Username => StaticValues.Username;
        public string Email => StaticValues.Email;
        public string FirstName => StaticValues.FirstName;
        public string LastName => StaticValues.LastName;

        public string AccountType => StaticValues.IsAdmin.Equals(true) ? "Administrator" : "Użytkownik";
    }
}