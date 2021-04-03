﻿using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Models;

namespace InventorySystem
{
    public static class Constants
    {
        //TODO: Change the "10.0.2.2" part to working address, as this one is a loopback to dev machine localhost
        public const string AccountEndpoint = "http://10.0.2.2:5000/api/account";
        public const string ItemsEndpoint = "http://10.0.2.2:5000/api/items";

        //Endpoints for specific things
        public const string AccountLogin = AccountEndpoint + "/login";
        public const string AccountRegister = AccountEndpoint + "/register";
    }
}