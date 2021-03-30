using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using InventorySystem.Models.CustomValidators;
using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Login
    {
        [Required, EmailAddress]
        [JsonProperty("email")]
        public static string Email { get; set; }
        [Required, Password]
        [JsonProperty("password")]
        public static string Password { get; set; }
    }
}
