using InventorySystem.Interfaces;
using InventorySystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.Services
{
    public class RestService
    {
        public const string Token = "token";
        public const string Connection_Connected = "Connected";
        public const string Connection_NoTokenFound = "NoTokenFound";
        public const string Connection_ConnectionError = "ConnectionError";
        public const string Connection_StatusFailure = "StatusFailure";
        public const string Connection_TokenExpired = "TokenExpired";

        private static HttpClient _client;

        private string _token;

        public RestService()
        {
            GetEndpoints();
            _client = new HttpClient
                {Timeout = TimeSpan.FromSeconds(30)};
            Items = null;
            Item = null;
        }

        private string AccountEndpoint { get; set; }
        private string ItemsEndpoint { get; set; }

        private Item Item { get; set; }
        private List<Item> Items { get; set; }

        public async Task<bool> VerifyLogin(Login valuesLogin)
        {
            var json = JsonConvert.SerializeObject(valuesLogin, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage;

            try
            {
                responseMessage = await _client.PostAsync(new Uri(AccountEndpoint + "/login"), content);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (!responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.UnauthorizedError, responseMessage.ToString());
                return false;
            }

            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

            if (userData == null)
            {
                ShowMessage(Constants.NullReturnedError, Constants.Console_NullReturnedError);
                return false;
            }

            SaveUserDetails(userData);
            await Application.Current.SavePropertiesAsync();
            await SecureStorage.SetAsync(Token, userData.Token);

            return true;
        }

        public async Task<bool> RegisterUser(Register valuesRegister)
        {
            var json = JsonConvert.SerializeObject(valuesRegister, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = null;

            try
            {
                responseMessage = await _client.PostAsync(new Uri(AccountEndpoint + "/register"), content);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.RegistrationError, responseMessage.ToString());
                return false;
            }

            if (responseMessage == null) return false;
            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonAsStringAsync);

            if (userData == null)
            {
                ShowMessage(Constants.NullReturnedError, Constants.Console_NullReturnedError);
                return false;
            }

            SaveUserDetails(userData);
            await Application.Current.SavePropertiesAsync();
            await SecureStorage.SetAsync(Token, userData.Token);
            return true;
        }

        public async Task<string> CheckConnection()
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return Connection_NoTokenFound;
                }

            HttpResponseMessage responseMessage = null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(AccountEndpoint));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return Connection_ConnectionError;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return Connection_ConnectionError;
            }

            if (responseMessage != null && responseMessage.IsSuccessStatusCode) return Connection_Connected;

            if (CheckIfTokenExpiredAndShowErrorMessage(responseMessage))
                return Connection_TokenExpired;

            ShowMessage(Constants.ApiRejectionError, responseMessage.ToString());
            return Connection_StatusFailure;
        }

        public async Task<List<Item>> GetAllItems()
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return null;
                }

            HttpResponseMessage responseMessage = null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(ItemsEndpoint));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return null;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
                throw;
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return null;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.ItemsError, responseMessage.ToString());
                return null;
            }

            if (responseMessage == null) return null;
            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);
            return Items;

        }

        //Methods used by ModifyItemPage
        public async Task<Item> GetSpecificItem(string id)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.NoTokenError);
                    return null;
                }

            HttpResponseMessage responseMessage = null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(ItemsEndpoint + $"/{id}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return null;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return null;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.SpecificItemError, responseMessage.ToString());
                return null;
            }

            if (responseMessage == null) return null;
            var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
            Item = JsonConvert.DeserializeObject<Item>(jsonAsStringAsync);
            return Item;
        }

        //
        public async Task<bool> UpdateItem(Item item)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            HttpResponseMessage responseMessage = null;

            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage =
                new HttpRequestMessage(HttpMethod.Put, new Uri(ItemsEndpoint + $"/{item.Id}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            requestMessage.Content = content;

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage != null && responseMessage.IsSuccessStatusCode) return true;

            ShowMessage(Constants.UpdateItemError, responseMessage?.ToString());
            return false;
            }
        //

        public async Task<List<Item>> GetScannedItem(string barcode)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return null;
                }

            HttpResponseMessage responseMessage = null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(ItemsEndpoint));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return null;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return null;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.SpecificItemError, responseMessage.ToString());
                return null;
            }

            if (responseMessage?.Content != null)
            {
                var jsonAsStringAsync = await responseMessage.Content.ReadAsStringAsync();
                Items = JsonConvert.DeserializeObject<List<Item>>(jsonAsStringAsync);
            }

            var matchingItems = Items?.FindAll(item => item.Barcode.Contains(barcode));

            return matchingItems;
        }

        public async Task<bool> DeleteItem(Guid itemId)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            HttpResponseMessage responseMessage = null;

            var requestMessage =
                new HttpRequestMessage(HttpMethod.Delete, new Uri(ItemsEndpoint + $"/{itemId}"));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (System.OperationCanceledException operationCanceledException)
            {
                ShowMessage(Constants.ConnectionError, operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.DeletionError, responseMessage.ToString());
                return false;
            }

            ShowMessage(Constants.DeletionSuccessful, responseMessage?.ToString());
            return true;
        }

        public async Task<bool> AddItem(Item item)
        {
            if (!CheckForToken())
                if (!await GetToken())
                {
                    ShowMessage(Constants.NoTokenError, Constants.Console_NoTokenError);
                    return false;
                }

            HttpResponseMessage responseMessage = null;

            var json = JsonConvert.SerializeObject(item, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(ItemsEndpoint));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            requestMessage.Content = content;

            try
            {
                responseMessage = await _client.SendAsync(requestMessage);
            }
            catch (TimeoutException toex)
            {
                ShowMessage(Constants.ConnectionError, toex.Message);
                return false;
            }
            catch (Exception ex)
            {
                ShowMessage(Constants.NotExpectedError, ex.Message);
                return false;
            }

            if (responseMessage != null && !responseMessage.IsSuccessStatusCode)
            {
                ShowMessage(Constants.AddingItemError, responseMessage.ToString());
                return false;
            }

            ShowMessage(Constants.AddingItemSuccessful, responseMessage?.ToString());
            return true;
        }

        //Helpers
        private void GetEndpoints()
        {
            AccountEndpoint = "https://newinventorymanagementsystemapi20211010162307.azurewebsites.net/api/account";
            ItemsEndpoint = "https://newinventorymanagementsystemapi20211010162307.azurewebsites.net/api/Items";
        }

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
            _token = await SecureStorage.GetAsync(Token);
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

            ShowMessage(Constants.ExpiredTokenError, response.ToString());
            return result;
        }

        private static void ShowInConsole(string message)
        {
            Debug.WriteLine("[REST Service] " + message);
        }

        private static void ShowMessage(string message, string consoleMessage)
        {
            DependencyService.Get<IMessage>().LongAlert(message);
            ShowInConsole(consoleMessage);
        }
    }
}