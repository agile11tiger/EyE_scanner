using Scanner.Services.Interfaces;
using SQLite;
using System.Collections.Generic;

namespace Scanner.Services
{
    /// <summary>
    /// Класс для взаимодействия с бд через SQLite
    /// </summary>
    //public class SQLiteDataBase : IDatabase
    //{
    //    public SQLiteDataBase(string databasePath)
    //    {
    //        database = new SQLiteConnection(databasePath);
    //    }

    //    private SQLiteConnection database;

    //    public CreateTableResult CreateTable<T>() where T : new()
    //    {
    //        return database.CreateTable<T>();
    //    }

    //    public IEnumerable<T> GetItems<T>() where T : new()
    //    {
    //        return database.Table<T>();
    //    }

    //    public T GetItem<T>(int id = 0) where T : new()
    //    {
    //        try
    //        {
    //            return database.Get<T>(id);
    //        }
    //        catch { return default; }
    //    }

    //    public int AddItem<T>(T item) where T : new()
    //    {
    //        return database.Insert(item);
    //    }

    //    public int RemoveItem<T>(int id = 0) where T : new()
    //    {
    //        try
    //        {
    //            return database.Delete<T>(id);
    //        }
    //        catch { return -1; }
    //    }
    //}
}
