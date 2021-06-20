using InventorySystem.Models;
using Xamarin.Forms;

namespace InventorySystem.Services
{
    public class UserItemsDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CurrentUser { get; set; }
        public DataTemplate OtherUsers { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (StaticValues.IsAdmin.Equals(true))
                return CurrentUser;
            return ((Item) item).User.Id.ToString().Equals(StaticValues.UserId) ? CurrentUser : OtherUsers;
        }
    }
}