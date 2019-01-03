using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.WindowsStore.AppStorage
{
    public static class AzureUrl
    {
        public static string GetUrl()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Read data from a simple setting

            Object value = localSettings.Values["AzureUrl"];

            if (value == null)
            {
                // Create a simple setting

                localSettings.Values["AzureUrl"] = "http://silveralerthellas.azurewebsites.net/api/missingservice/searchmissings";
            }
            else
            {
                value = localSettings.Values["AzureUrl"];
            }

            return value.ToString();
        }

        public static string GetPictureBlob()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Read data from a simple setting

            Object value = localSettings.Values["PictureBlob"];

            if (value == null)
            {
                // Create a simple setting

                localSettings.Values["PictureBlob"] = "http://slhellas.blob.core.windows.net/pictures/";

                value = localSettings.Values["PictureBlob"];
            }
            else
            {
                value = localSettings.Values["PictureBlob"];
            }

            return value.ToString();
        }
    }
}
