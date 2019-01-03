using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using SilverAlert.Shared;
using Windows.Networking.Connectivity;
using System.Net;
using System.IO;
using SilverAlert.WindowsStore.Connectivity;
using System.Diagnostics;
using SilverAlert.WindowsStore.AppStorage;
using Windows.UI.Xaml.Media.Imaging;

namespace SilverAlert.WindowsStore
{
    public class FileManagement
    {
        public static async void SaveFile(string FileName, string newData)
        {
            try{
            
            StorageFile dataFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName,
                CreationCollisionOption.OpenIfExists);

            String jsonData = await FileIO.ReadTextAsync(dataFile);

            if (String.IsNullOrEmpty(jsonData) || jsonData == "")
            {
                await FileIO.WriteTextAsync(dataFile, newData);
            }
            else if (!String.IsNullOrEmpty(jsonData) || jsonData != "")
            {
                await FileIO.AppendTextAsync(dataFile, "," + newData);
            }
             
            }
            catch (Exception)
            {
                
            }

        }

        public async static Task<string> ReadFile(string FileName)
        {
            try{
            StorageFile dataFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(FileName,
                CreationCollisionOption.OpenIfExists);

            String jsonData = await FileIO.ReadTextAsync(dataFile);
            return jsonData;
            }
            catch (Exception)
            {
                return "";
            }

            
        }


        public static async void DeleteFile(string FileName)
        {
            try
            {

            StorageFile dataFile = await ApplicationData.Current.LocalFolder.GetFileAsync(FileName);

            if (dataFile != null)
            {
                await dataFile.DeleteAsync();

            }

            }
            catch (Exception)
            {

            }

        }



        /// <summary>
        /// Copies an image from the internet (http protocol) locally to the AppData LocalFolder.  This is used by some methods 
        /// (like the SecondaryTile constructor) that do not support referencing images over http but can reference them using 
        /// the ms-appdata protocol.  
        /// </summary>
        /// <param name="internetUri">The path (URI) to the image on the internet</param>
        /// <param name="uniqueName">A unique name for the local file</param>
        /// <returns>Path to the image that has been copied locally</returns>
        public static async void SaveImageAsync(string imageName)
        {
            try
            {
                using (var response = await HttpWebRequest.CreateHttp(AzureUrl.GetPictureBlob() + imageName.ToLowerInvariant()).GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var desiredName = string.Format("{0}", imageName);

                        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);

                        using (var filestream = await file.OpenStreamForWriteAsync())
                        {
                            await stream.CopyToAsync(filestream);
                        }


                    }
                }
            }
            catch (Exception)
            {

            }


        }
    }
}
