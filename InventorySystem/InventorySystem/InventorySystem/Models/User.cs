using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models
{
    class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<Item> Items { get; set; }
    }
}
