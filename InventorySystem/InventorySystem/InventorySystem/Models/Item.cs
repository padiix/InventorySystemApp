using System;
using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Item
    {
        public Item(Guid id, string name, string barcode, DateTimeOffset dateAdded)
        {
            Id = id;
            Name = name;
            Barcode = barcode;
            DateAdded = dateAdded;
        }

        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("dateAdded")] public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("barcode")] public string Barcode { get; set; }

        [JsonProperty("user")] public User User { get; set; }
    }
}