using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace InventorySystem.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public string Barcode { get; set; }
        //public string UserId { get; set; }
        public User User { get; set; }
    }
}
