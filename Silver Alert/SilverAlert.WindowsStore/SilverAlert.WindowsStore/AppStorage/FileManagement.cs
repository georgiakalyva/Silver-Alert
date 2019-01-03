using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SilverAlert.Lib;
using Windows.Networking.Connectivity;

namespace SilverAlert.WindowsStore.AppStorage
{
    public class FileManagement
    {
        async void ReadFile(string dataFolder)
        {
            StorageFile dataFile = await StorageSettings.LocalStorageFolder.CreateFileAsync(dataFolder,
                CreationCollisionOption.OpenIfExists);

            String jsonData = await FileIO.ReadTextAsync(dataFile);

            if (String.IsNullOrEmpty(jsonData))
            {
                //Download From Site
            }
            
            List<MissingPerson> MissingPeople = new List<MissingPerson>();
            List<MissingPerson> UknownPeople = new List<MissingPerson>();
            

        }

        async void RefreshFile(string dataFolder)
        {
            StorageFile dataFile = 
                await StorageSettings.LocalStorageFolder.GetFileAsync(dataFolder);

            String jsonData = await FileIO.ReadTextAsync(dataFile);


            if (String.IsNullOrEmpty(jsonData))
            {
                //Download From Site
            }

            List<MissingPerson> MissingPeople = new List<MissingPerson>();
            List<MissingPerson> UknownPeople = new List<MissingPerson>();

            
        }
    }
}
