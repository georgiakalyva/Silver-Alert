using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SilverAlert.WindowsStore.AppStorage
{
    public class StorageSettings
    {
        public static StorageFolder LocalStorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public const string filename = "SilverAlertData.txt";
    }
}
