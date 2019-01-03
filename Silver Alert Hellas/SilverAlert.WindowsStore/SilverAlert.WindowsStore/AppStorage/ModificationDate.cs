using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.WindowsStore.AppStorage
{
    public class ModificationDate
    {
        public static string GetValue()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Read data from a simple setting

            Object ID = localSettings.Values["ModificationDate"];

            if (ID == null)
            {
                // Create a simple setting
                return "";
            }
            //localSettings.Values.Remove("exampleSetting");
            return localSettings.Values["ModificationDate"].ToString();
        }
        public static void SaveValue(string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            localSettings.Values.Remove("ModificationDate");
            localSettings.Values["ModificationDate"] = value;

        }

        public static void RemoveValue()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            localSettings.Values.Remove("ModificationDate");

        }
    }
}
