using SilverAlert.DataModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SilverAlert.Data
{
    public static class Database
    {
        public static async Task<bool> Exists(string DatabaseName)
        {
            bool dbexist = true;

            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(DatabaseName);

            }
            catch
            {
                dbexist = false;
            }

            return dbexist;
        }

        public static void Open(string DatabaseName)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(DatabaseName);
        }

        public static async Task<bool> DeleteDatabase(string DatabaseName)
        {
            try
            {
                StorageFile dbfile = await ApplicationData.Current.LocalFolder.GetFileAsync(DatabaseName);
                await dbfile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async void CreateTables(string DatabaseName)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(DatabaseName);

            await connection.CreateTableAsync<Language>();
            await connection.CreateTableAsync<EyeColor>();
            await connection.CreateTableAsync<BodyType>();
            await connection.CreateTableAsync<Category>();
            await connection.CreateTableAsync<City>();
            await connection.CreateTableAsync<MissingPerson>();
        }

    }
}
