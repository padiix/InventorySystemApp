using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace InventorySystem.Models
{
    public class Item
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("dateAdded")]
        public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("barcode")]
        public string Barcode { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
