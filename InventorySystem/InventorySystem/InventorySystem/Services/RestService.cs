using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;
using InventorySystem.ViewModels;

namespace InventorySystem.Services
{
    class RestService
    {
        HttpClient _client;

        public RestService()
        {
            _client = new HttpClient();

            //TODO: Implement connectivity with the API
        }
    }
}
