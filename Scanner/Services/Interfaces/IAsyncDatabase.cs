using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scanner.Services.Interfaces
{
    /// <summary>
    /// Интерфейс для взаимодействия с бд асинхронно
    /// </summary>
    public interface IAsyncDatabase
    {
        Task<CreateTableResult> CreateTableAsync<T>() where T : new();
        Task<List<T>> GetItemsAsync<T>() where T : new();
        Task<T> GetItemAsync<T>(int id = 0) where T : new();
        Task<int> AddItemAsync<T>(T item) where T : new();
        Task<int> AddOrReplaceItemAsync<T>(T item) where T : new();
        Task<int> RemoveItemAsync<T>(int id = 0) where T : new();
    }
}
