using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Register
    {
        public Register(string username, string firstname, string lastname, string email, string password)
        {
            Username = username;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Password = password;
        }

        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("firstname")]
        public string Firstname { get; set; }
        [JsonProperty("lastname")]
        public string Lastname { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}