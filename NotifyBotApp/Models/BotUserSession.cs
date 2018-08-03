using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifyBotApp.Models
{
    public class BotUserSession : TableEntity
    {
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public object Id { get; set; }

        public string toId { get; set; }
        public string toName { get; set; }
        public string fromId { get; set; }
        public string fromName { get; set; }
        public string channelId { get; set; }
        public string conversationId { get; set; }
        public string serviceUrl { get; set; }

        public bool active { get; set; }
    }
}