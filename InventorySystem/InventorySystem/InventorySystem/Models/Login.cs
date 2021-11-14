using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Login
    {
        public Login(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}