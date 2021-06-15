using InventorySystem.Interfaces;
using InventorySystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.ViewModels;
using InventorySystem.Views;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace InventorySystem.Services
{
    public class RestService : IRestService
    {
        //TODO: Simplify as much as possible how request are working
        // 
        // Methods already done:
        // ...
        //
        // Methods to improve:
        // All of them
        //

        public const string Token = "token";
        public const string Connection_Connected = "Connected";
        public const string Connection_NoTokenFound = "NoTokenFound";
        public const string Connection_ConnectionError = "ConnectionError";
        public const string Connection_StatusFailure = "StatusFailure";
        public const string Connection_TokenExpired = "TokenExpired";

        private string _token;

        private static HttpClient _client;

        private Item Item { get; set; }
        private List<Item> Items { get; set; }

        public RestService()
        {
            _client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) }; //Po 10 sekundach HttpClient zwróci problem z po³¹czeniem.
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
            catch (TimeoutException toex)
            {
                ShowErrorMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                ShowErrorMessage(Constants.UnauthorizedError, responseMessage.ToString());
                return false;
            }

            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

            if (userData == null)
            {
                ShowErrorMessage(Constants.NullReturnedError, Constants.Console_NullReturnedError);
                return false;
            }

            SaveUserDetails(userData);
            await Xamarin.Essentials.SecureStorage.SetAsync(Token, userData.Token);

            await Application.Current.SavePropertiesAsync();
            return true;
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
            catch (TimeoutException toex)
            {
                ShowErrorMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                ShowErrorMessage(Constants.RegistrationError, responseMessage.ToString());
                return false;
            }

            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

            if (userData == null)
            {
                ShowErrorMessage(Constants.NullReturnedError, Constants.Console_NullReturnedError);
                return false;
            }

            SaveUserDetails(userData);
            await Xamarin.Essentials.SecureStorage.SetAsync(Token, userData.Token);

            await Application.Current.SavePropertiesAsync();
            return true;
        }

        public async Task<string> CheckConnection()
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return Connection_NoTokenFound;
                }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.AccountEndpoint)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                HttpResponseMessage responseMessage;

                try
                {
                    responseMessage = await _client.SendAsync(requestMessage);
                }
                catch (TimeoutException toex)
                {
                    ShowErrorMessage(Constants.ConnectionError, toex.Message);
                    return Connection_ConnectionError;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                    return Connection_ConnectionError;
                }

                if (responseMessage.IsSuccessStatusCode) return Connection_Connected;

                if (CheckIfTokenExpiredAndShowErrorMessage(responseMessage))
                    return Connection_TokenExpired;

                ShowErrorMessage(Constants.ApiRejectionError, responseMessage.ToString());
                return Connection_StatusFailure;
            }
        }
        public async Task<List<Item>> GetAllItems()
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return null;
                }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(Constants.ItemsEndpoint)))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                HttpResponseMessage responseMessage;

                try
                {
                    responseMessage = await _client.SendAsync(requestMessage);
                }
                catch (TimeoutException toex)
                {
                    ShowErrorMessage(Constants.ConnectionError, toex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                    return null;
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    ShowErrorMessage(Constants.ItemsError, responseMessage.ToString());
                    return null;
                }

                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);

                return Items;
            }
        }

        //Methods used by ModifyItemPage
        public async Task<Item> GetSpecificItem(string id)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.NoTokenError);
                    return null;
                }

            var uri = new Uri(Constants.ItemsEndpoint + "/" + id);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                HttpResponseMessage responseMessage;
                try
                {
                    responseMessage = await _client.SendAsync(requestMessage);
                }
                catch (TimeoutException toex)
                {
                    ShowErrorMessage(Constants.ConnectionError, toex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                    return null;
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    ShowErrorMessage(Constants.SpecificItemError, responseMessage.ToString());
                    return null;
                }

                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                Item = JsonConvert.DeserializeObject<Item>(jsonAsStringAsync);

                return Item;
            }
        }
        public async Task<bool> UpdateItem(Guid itemId, Item item)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            HttpResponseMessage responseMessage;

            var uri = Constants.ItemsEndpoint + $"/{itemId}";
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, uri);

            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            requestMessage.Content = content;

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowErrorMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage.IsSuccessStatusCode) return true;
            
            ShowErrorMessage(Constants.UpdateItemError, responseMessage.ToString());
            return false;
        }
        //

        public async Task<List<Item>> GetScannedItem(string barcode)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return null;
                }

            var uri = new Uri(Constants.ItemsEndpoint);

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                HttpResponseMessage responseMessage;
                try
                {
                    responseMessage = await _client.SendAsync(requestMessage);
                }
                catch (TimeoutException toex)
                {
                    ShowErrorMessage(Constants.ConnectionError, toex.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                    return null;
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    ShowErrorMessage(Constants.SpecificItemError, responseMessage.ToString());
                    return null;
                }

                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);

                var matchingItems = Items?.FindAll(item => item.Barcode.Contains(barcode));

                return matchingItems;
            }
        }
        public async Task<bool> DeleteItem(Guid itemId)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, new Uri(Constants.ItemsEndpoint + $"/{itemId}")))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                HttpResponseMessage responseMessage;

                try
                {
                    responseMessage = await _client.SendAsync(requestMessage);
                }
                catch (TimeoutException toex)
                {
                    ShowErrorMessage(Constants.ConnectionError, toex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                    return false;
                }

                if (!responseMessage.IsSuccessStatusCode)
                {
                    ShowErrorMessage(Constants.DeletionError, responseMessage.ToString());
                    return false;
                }

                ShowErrorMessage(Constants.DeletionSuccessful, responseMessage.ToString());
                return true;
            }
        }
        public async Task<bool> AddItem(Item item)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowErrorMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            HttpResponseMessage responseMessage;

            var uri = Constants.ItemsEndpoint;
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            requestMessage.Content = content;

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowErrorMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowErrorMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage.IsSuccessStatusCode) return true;

            ShowErrorMessage(Constants.AddingItemError, responseMessage.ToString());
            return false;
        }

        //Additional usefull methods
        private static void SaveUserDetails(UserData userData)
        {
            StaticValues.UserId = userData.Id.ToString();
            StaticValues.FirstName = userData.FirstName;
            StaticValues.LastName = userData.LastName;
            StaticValues.Username = userData.Username;
            StaticValues.Email = userData.Email;
            StaticValues.IsAdmin = userData.IsAdmin;
        }
        private async Task<bool> GetToken()
        {
            _token = await Xamarin.Essentials.SecureStorage.GetAsync(Token);
            return CheckForToken();
        }
        private bool CheckForToken()
        {
            if (!string.IsNullOrWhiteSpace(_token)) return true;
            return false;
        }
        private bool CheckIfTokenExpiredAndShowErrorMessage(HttpResponseMessage response)
        {
            var headers = response.Headers.WwwAuthenticate.GetEnumerator();
            headers.MoveNext();
            var header = headers.Current;
            headers.Dispose();
            var result = header != null &&
                         header.Scheme.Contains("Bearer") &&
                         header.Parameter.Contains("error=\"invalid_token\", error_description=\"The token expired at");

            ShowErrorMessage(Constants.ExpiredTokenError, response.ToString());
            return result;
        }
        private static void ShowInConsole(string message)
        {
            Console.WriteLine("[API Error Message] " + message);
        }
        private void ShowErrorMessage(string errorMessage , string consoleMessage)
        {
            DependencyService.Get<IMessage>().LongAlert(errorMessage);
            ShowInConsole(consoleMessage);
        }
    }
}
