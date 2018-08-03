using MongoDB.Driver;

namespace NotifyBot.Interfaces
{
    public interface IDBAccess
    {
        string ConnectionString { get; set; }
        MongoClient GetMongoClient();
    }
}