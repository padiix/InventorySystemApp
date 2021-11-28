using System;
using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class User
    {
        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("firstName")] public string FirstName { get; set; }

        [JsonProperty("lastName")] public string LastName { get; set; }

        [JsonProperty("email")] public string Email { get; set; }

        public string FirstAndLastName
        {
            get => FirstName + " " + LastName;
        }
    }
}