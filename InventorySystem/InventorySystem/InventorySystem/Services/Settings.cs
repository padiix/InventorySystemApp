using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace InventorySystem.Services
{
    public static class Settings
    {
        public static bool FirstRun
        {
            get => Preferences.Get(nameof(FirstRun), true);
            set => Preferences.Set(nameof(FirstRun), value);
        }

        public static bool RememberMe
        {
            get => Preferences.Get(nameof(RememberMe), false);
            set => Preferences.Set(nameof(RememberMe), value);
        }
    }
}
