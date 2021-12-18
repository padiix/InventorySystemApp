using System;
using System.ComponentModel.Design;
using Newtonsoft.Json;

namespace InventorySystem.Models
{
    public class Item
    {
        public Item(Guid id, string name, string description, string barcode, DateTimeOffset dateAdded)
        {
            Id = id;
            Name = name;
            Description = string.IsNullOrEmpty(description) ? "Brak" : description;
            Barcode = barcode;
            DateAdded = dateAdded;
        }

        [JsonProperty("id")] public Guid Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("dateAdded")] public DateTimeOffset DateAdded { get; set; }

        [JsonProperty("barcode")] public string Barcode { get; set; }

        [JsonProperty("user")] public User User { get; set; }

        public string FormattedBarcode
        {
            get
            {
                var output = Barcode[0] + " ";
                for (var i = 1; i < Barcode.Length; i++)
                {
                    var modulo = i % 6;
                    if (modulo == 0)
                    {
                        output += Barcode[i] + " ";
                    }
                    else
                    {
                        output += Barcode[i];
                    }
                }

                return output;
            }
        }
    }
}