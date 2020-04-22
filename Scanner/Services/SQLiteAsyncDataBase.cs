using Scanner.Services.Interfaces;
using SQLite;
using System;
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

        private readonly SQLiteAsyncConnection database;

        public async Task<CreateTableResult> CreateTableAsync<T>() where T : new()
        {
            return await database.CreateTableAsync<T>();
        }

        public async Task<List<T>> GetItemsAsync<T>() where T : new()
        {
            return await database.Table<T>().ToListAsync();
        }

        public async Task<T> GetItemAsync<T>(int id = 0) where T : new()
        {
            try
            {
                return await database.GetAsync<T>(id);
            }
            catch { return default; }
        }

        public async Task<int> AddItemAsync<T>(T item) where T : new()
        {
            return await database.InsertAsync(item);
        }

        public async Task<int> AddOrReplaceItemAsync<T>(T item) where T : new()
        {
            //не понятно почему, но этот метод не увеличивает primarykey(из за этого заменяет старый объект)
            return await database.InsertOrReplaceAsync(item);
        }

        public async Task<int> RemoveItemAsync<T>(int id = 0) where T : new()
        {
            try
            {
                return await database.DeleteAsync<T>(id);
            }
            catch { return await Task.FromResult(-1); }
        }
    }
}
