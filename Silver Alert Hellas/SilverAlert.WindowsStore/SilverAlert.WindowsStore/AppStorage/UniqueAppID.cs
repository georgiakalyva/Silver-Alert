using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.WindowsStore.AppStorage
{
    public static class UniqueAppID
    {
        public static string SetValue(string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Read data from a simple setting

            Object ID = localSettings.Values["UniqueID"];

            if (ID==null)
            {
                // Create a simple setting
                localSettings.Values["UniqueID"] = value;
            }
            //localSettings.Values.Remove("exampleSetting");
            return localSettings.Values["UniqueID"].ToString();
        }
      
    }
}
