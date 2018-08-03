using System;
using System.Collections.Generic;
using System.Security.Authentication;
using MongoDB.Driver;
using NotifyBot.Interfaces;
using Serilog;
using NotifyBotApp.Models;

namespace NotifyBotApp.DataAccess
{
    public class DBAccess : IDBAccess
    {
        private string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public MongoClient GetMongoClient()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(_connectionString)
            );

            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            var mongoClient = new MongoClient(settings);
            return mongoClient;
        }

        internal void SaveBotUserSessionData(BotUserSession data)
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                Log.Warning("No Connection String given, ignore persistance");
                return;
            }
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            colDataPoints.InsertOne(data);
        }

        internal void UpdateBotUserSessionData(BotUserSession data)
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                Log.Warning("No Connection String given, ignore persistance");
                return;
            }
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            var filter = Builders<BotUserSession>.Filter.Eq("Id", data.Id);
            var update = Builders<BotUserSession>.Update.Set("active", data.active);

            colDataPoints.UpdateOne(filter, update);
        }

        internal List<BotUserSession> GetBotUserSessionData()
        {
            if (String.IsNullOrEmpty(_connectionString))
            {
                Log.Warning("No Connection String given, ignore persistance");
                return new List<BotUserSession>();
            }
            var mongoClient = GetMongoClient();
            var mongoDB = mongoClient.GetDatabase("botsessions");

            var colDataPoints = mongoDB.GetCollection<BotUserSession>("botsessions");
            var documents = colDataPoints.Find<BotUserSession>(o => true);
            return documents.ToList<BotUserSession>();
        }
    }
}

