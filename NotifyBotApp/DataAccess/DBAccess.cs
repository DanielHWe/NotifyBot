using System.Collections.Generic;
using System.Security.Authentication;
using MongoDB.Driver;
using WhereIsMyBikeBotApp.Models;

namespace WhereIsMyBikeBotApp.DataAccess
{
    public static class DBAccess
    {
        private static string _connectionString;

        internal static string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        private static MongoClient GetMongoClient()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(_connectionString)
    );

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);
            return mongoClient;
        }

        public static void SaveBotUserSessionData(BotUserSession data)
        {
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            colDataPoints.InsertOne(data);
        }

        public static void UpdateBotUserSessionData(BotUserSession data)
        {
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            var filter = Builders<BotUserSession>.Filter.Eq("Id", data.Id);
            var update = Builders<BotUserSession>.Update.Set("active", data.active);

            colDataPoints.UpdateOne(filter, update);
        }

        public static List<BotUserSession> GetBotUserSessionData()
        {
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            var documents = colDataPoints.Find<BotUserSession>(o => true);
            return documents.ToList<BotUserSession>();
        }
    }
}

