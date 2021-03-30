using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models
{
    public class PublicUserViewModel
    {
        public string Token { get; set; }
        public static string Username { get; set; }
        public static string Email { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
    }
}
