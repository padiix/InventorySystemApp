using System;
using System.Collections.Generic;
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

        public Item Items { get; set; }
        readonly HttpClient _client;
        private PublicUserViewModel _publicUser;
        public const string Token = "token";

        public RestService()
        {
            _client = new HttpClient();

            //TODO: Implement all of the methods for working with API
        }

        public async Task<bool> VerifyLogin()
        {
            var uri = new Uri(Constants.AccountEndpoint + "/login");
            var valuesLogin = new Login();


            try
            {
                var json = JsonConvert.SerializeObject(valuesLogin, Formatting.Indented);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage resp = null;

                resp = await _client.PostAsync(uri, content);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await resp.Content.ReadAsStringAsync();
                    _publicUser = JsonConvert.DeserializeObject<PublicUserViewModel>(jsonAsStringAsync);
                    await Xamarin.Essentials.SecureStorage.SetAsync(RestService.Token, _publicUser.Token);
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

        public async Task<Item> GetItems()
        {
            var uri = new Uri(Constants.ItemsEndpoint);

            try
            {
                HttpResponseMessage resp = null;

                resp = await _client.GetAsync(uri);

                if (resp.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await resp.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<Item>(jsonAsStringAsync);
                    await Xamarin.Essentials.SecureStorage.SetAsync(RestService.Token, _publicUser.Token);
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
