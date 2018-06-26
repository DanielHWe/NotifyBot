using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyBot.Interfaces
{
    public class NotifyBotSettings
    {
        public String BotId { get; set; }
        public String MicrosoftAppId { get; set; }
        public String MicrosoftAppPassword { get; set; }
        public String MongoDbConnectionString { get; set; }
    }
}
