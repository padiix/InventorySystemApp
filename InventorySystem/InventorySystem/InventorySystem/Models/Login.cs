using System.ComponentModel.DataAnnotations;
using InventorySystem.Models.CustomValidators;
using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [Password]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}