using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventorySystem.Interfaces;
using InventorySystem.Models;
using Xamarin.Forms;
using InventorySystem.ViewModels;
using Newtonsoft.Json;
using Xamarin.Forms.Internals;

namespace InventorySystem.Services
{
    class RestService : IRestService
    {

        public List<Item> Items { get; set; }
        private readonly HttpClient _client;
        private UserData _userData;
        public const string Token = "token";

        public RestService()
        {
            _client = new HttpClient();
            Items = new List<Item>();
            //TODO: Implement all of the methods for working with API
        }

        public async Task<bool> VerifyLogin(string email, string password)
        {
            var uri = new Uri(Constants.AccountLogin);
            var valuesLogin = new Login()
            {
                Email = email,
                Password = password
            };

            try
            {
                var json = JsonConvert.SerializeObject(valuesLogin, Formatting.Indented);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage resp = null;

                resp = await _client.PostAsync(uri, content);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await resp.Content.ReadAsStringAsync();
                    _userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

                    await Xamarin.Essentials.SecureStorage.SetAsync(Token, _userData.Token);

                    StaticValues.UserId = _userData.Id.ToString();
                    StaticValues.FirstName = _userData.FirstName;
                    StaticValues.LastName = _userData.LastName;
                    StaticValues.Username = _userData.Username;
                    StaticValues.Email = _userData.Email;

                    if (!Settings.RememberMe) return true;

                    await Application.Current.SavePropertiesAsync();

                    return true;
                }
                else
                {
                    var errorMessage = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine(errorMessage);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        public async Task<List<Item>> GetItems()
        {
            var uri = new Uri(Constants.ItemsEndpoint);

            try
            {
                HttpResponseMessage resp = null;

                resp = await _client.GetAsync(uri);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await resp.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);
                    
                    return Items;
                }
                else
                {
                    var errorMessage = await resp.Content.ReadAsStringAsync();
                    Debug.WriteLine(errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
