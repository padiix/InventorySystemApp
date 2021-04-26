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
        
        private Item Item { get; set; }
        private List<Item> Items { get; set; }
        private UserData _userData;

        public RestService()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            Items = null;
            Item = null;
        }

        public async Task<bool> VerifyLogin(string email, string password)
        {
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
                responseMessage = await _client.PostAsync(new Uri(Constants.AccountLogin), content);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                _userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

                await Xamarin.Essentials.SecureStorage.SetAsync(Token, _userData.Token);

                return true;
            }
            else
            {
                //throw new Exception(Constants.UnauthorizedError);
                DependencyService.Get<IMessage>().LongAlert(Constants.UnauthorizedError);
                return false;
            }
        }

        public async Task<bool> Register(string username, string firstname, string lastname, string email, string password)
        {
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
                responseMessage = await _client.PostAsync(new Uri(Constants.AccountRegister), content);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                DependencyService.Get<IMessage>().LongAlert(Constants.NoTokenError);
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
                    throw new Exception(ex.Message);
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
            var token = await Xamarin.Essentials.SecureStorage.GetAsync(Token);

            if (string.IsNullOrWhiteSpace(token))
            {
                DependencyService.Get<IMessage>().LongAlert(Constants.NoTokenError);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ItemsEndpoint)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;

                try
                {
                    response = await _client.SendAsync(requestMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await response.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);

                    return Items;
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine(errorMessage);
                return null;
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
                    throw new Exception(ex.Message);
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

        //TODO: Check if DeleteItem works
        public async Task<bool> DeleteItem(Guid itemId)
        {
            var token = await Xamarin.Essentials.SecureStorage.GetAsync(Token);

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new Exception(Constants.NoTokenError);
            }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri(Constants.ItemsEndpoint + $"/{itemId}")))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;

                try
                {
                    response = await _client.SendAsync(requestMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonAsStringAsync = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Rest Client: {jsonAsStringAsync}");
                    DependencyService.Get<IMessage>().LongAlert(Constants.DeletionSuccessful);
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorMessage);
                    return false;
                }
            }
        }
    }
}
