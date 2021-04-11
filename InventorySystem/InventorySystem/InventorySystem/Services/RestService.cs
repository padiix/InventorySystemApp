using InventorySystem.Interfaces;
using InventorySystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventorySystem.Services
{
    public class RestService : IRestService
    {
        public const string Token = "token";

        private readonly HttpClient _client;
        
        public Item Item { get; set; }
        public List<Item> Items { get; set; }
        private UserData _userData;

        public RestService()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            Items = null;
            Item = null;
        }

        public async Task<bool> VerifyLogin(string email, string password)
        {
            var uri = new Uri(Constants.AccountLogin);
            var valuesLogin = new Login()
            {
                Email = email,
                Password = password
            };

            var json = JsonConvert.SerializeObject(valuesLogin, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await _client.PostAsync(uri, content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //DependencyService.Get<IMessage>().LongAlert($"Error: {ex.Message}");
                return false;
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                _userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

                await Xamarin.Essentials.SecureStorage.SetAsync(Token, _userData.Token);

                return true;
            }

            var errorMessage = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine(errorMessage);

            return false;
        }

        public async Task<bool> Register(string username, string firstname, string lastname, string email, string password)
        {
            var uri = new Uri(Constants.AccountRegister);
            var valuesRegister = new Register()
            {
                Email = email,
                FirstName = firstname,
                LastName = lastname,
                Password = password,
                UserName = username
            };

            var json = JsonConvert.SerializeObject(valuesRegister, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await _client.PostAsync(uri, content);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }

            if (!responseMessage.IsSuccessStatusCode) return false;

            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            _userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

            await Xamarin.Essentials.SecureStorage.SetAsync(Token, _userData.Token);

            return true;
        }

        public async Task<bool> GetCurrentUser()
        {
            var token = await Xamarin.Essentials.SecureStorage.GetAsync(Token);

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.AccountEndpoint)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;

                try
                {
                    response = await _client.SendAsync(requestMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //DependencyService.Get<IMessage>().LongAlert($"Error: {ex.Message}");
                    return false;
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await response.Content.ReadAsStringAsync();
                    _userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

                    StaticValues.UserId = _userData.Id.ToString();
                    StaticValues.FirstName = _userData.FirstName;
                    StaticValues.LastName = _userData.LastName;
                    StaticValues.Username = _userData.Username;
                    StaticValues.Email = _userData.Email;

                    await Application.Current.SavePropertiesAsync();
                    return true;
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorMessage);

                return false;
            }
        }

        public async Task<List<Item>> GetAllItems()
        {
            var uri = new Uri(Constants.ItemsEndpoint);

            try
            {
                HttpResponseMessage response = null;

                response = await _client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await response.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);

                    return Items;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        //TODO: Check if GetSpecificItem works
        public async Task<Item> GetSpecificItem(string barcode)
        {
            var token = await Xamarin.Essentials.SecureStorage.GetAsync(Token);
            var uri = new Uri(Constants.ItemsEndpoint + "/" + barcode);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(requestMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await response.Content.ReadAsStringAsync();
                    Item = JsonConvert.DeserializeObject<Item>(jsonAsStringAsync);
                    return Item;
                }
                Console.WriteLine(response.Content.ReadAsStringAsync());
                return null;
            }
        }
    }
}
