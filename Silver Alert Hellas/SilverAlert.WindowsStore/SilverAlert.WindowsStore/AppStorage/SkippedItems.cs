using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilverAlert.WindowsStore.AppStorage
{
    public class SkippedItems
    {
        public static string Merge(int[] FoundPeople)
        {
            try
            {

            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            // Read data from a simple setting

            Object ID = localSettings.Values["SkippedItems"];

            String AppendString = string.Join(",", FoundPeople);            

            if (ID == null)
            {
                // Create a simple setting
                localSettings.Values["SkippedItems"] = AppendString;
            }
            else
            {
                String Text = localSettings.Values["SkippedItems"].ToString();
                localSettings.Values.Remove("SkippedItems");

                localSettings.Values["SkippedItems"] = Text + "," + AppendString;
            }
                    return localSettings.Values["SkippedItems"].ToString();
            }
            catch (Exception)
            {
                return "";
            }
            
            
        }
        public static string Get()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Object ID = localSettings.Values["SkippedItems"];
            if (ID == null)
            {
                return "";
            }
            else
            {
                return localSettings.Values["SkippedItems"].ToString();
            }
        }
        public static void Delete()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            object ID = localSettings.Values["SkippedItems"];

            if (ID != null)
            {
                localSettings.Values.Remove("SkippedItems");
            }

        }
    }
}
