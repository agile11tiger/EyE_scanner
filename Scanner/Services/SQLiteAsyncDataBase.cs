using Scanner.Services.Interfaces;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scanner.Services
{
    /// <summary>
    /// Класс для взаимодействия с бд через SQLite асинхронно
    /// </summary>
    public class SQLiteAsyncDataBase : IAsyncDatabase
    {
        public SQLiteAsyncDataBase(string databasePath)
        {
            database = new SQLiteAsyncConnection(databasePath);
        }

        private SQLiteAsyncConnection database;

        public Task<CreateTableResult> CreateTableAsync<T>() where T : new()
        {
            return database.CreateTableAsync<T>();
        }

        public Task<List<T>> GetItemsAsync<T>() where T : new()
        {
            return database.Table<T>().ToListAsync();
        }

        public Task<T> GetItemAsync<T>(int id = 0) where T : new()
        {
            try
            {
                return database.GetAsync<T>(id);
            }
            catch { return default; }
        }

        public Task<int> AddItemAsync<T>(T item) where T : new()
        {
            return database.InsertAsync(item);
        }

        public Task<int> RemoveItemAsync<T>(int id = 0) where T : new()
        {
            try
            {
                return database.DeleteAsync<T>(id);
            }
            catch { return Task.FromResult(-1); }
        }
    }
}
