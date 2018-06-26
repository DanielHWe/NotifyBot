using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using WhereIsMyBikeBotApp.DataAccess;

namespace WhereIsMyBikeBotApp.Models
{
    internal class NotificationManager
    {
        private static System.Collections.Generic.Dictionary<String, NotificationManager> _starter = new System.Collections.Generic.Dictionary<string, NotificationManager>();
        public BotUserSession Session { get; set; }
        public static String LastMessage { get; set; } = "No Last message";

        static NotificationManager() {
            try
            {
                //var table = GetCloudTable();
                //TableQuery<BotUserSession> query = new TableQuery<BotUserSession>();

                foreach (BotUserSession entity in DBAccess.GetBotUserSessionData())
                {
                    var starter = new NotificationManager();
                    starter.Session = entity;
                    
                    try
                    {
                        var t = starter.NotifyThisAsync("Service Back Online");
                        t.Start();
                        t.Wait();
                        _starter.Add(entity.toId + entity.toName, starter);
                    } catch (Exception ex2){
                        Log.Error(ex2, "Remove Stored: " + entity.toId + ": " + ex2.Message);
                    }
                }

                Log.Debug("Load sessions finished.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Load: " + ex.Message + " - " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException, "Error Load: " + ex.InnerException.Message);
                }
            }
        }

        private static BotUserSession GetUserSession(IMessageActivity message)
        {
            var key = GetKey(message);
            if (!_starter.ContainsKey(key))
            {
                if (!_starter.ContainsKey(message.From.Id + message.From.Name + message.Recipient.Name))
                {
                    return null;
                }
                return _starter[message.From.Id + message.From.Name].Session;
            }
            return _starter[message.Recipient.Id + message.Recipient.Name].Session;
        }

        internal static String GetUserAsString(IMessageActivity message)
        {
            var user = GetUserSession(message);
            if (user == null)
            {
                return "No user with Recipient id " + message.Recipient.Id + " or From id " + message.From.Id + " found";
            }
            StringBuilder result = new StringBuilder();
            AddUserAsReadAbleString(user, result);
            return result.ToString();
        }

        internal static String SetUserActive(IMessageActivity message, bool newActiveValue)
        {
            var user = GetUserSession(message);
            if (user == null)
            {
                return "No user with Recipient id " + message.Recipient.Id + " or From id " + message.From.Id + " found";
            }
            user.active = newActiveValue;
            try
            {
                DBAccess.SaveBotUserSessionData(user);
                return newActiveValue ? "Du hast den Cannal abboniert" : "Du wirst nicht mehr benachtichtigt";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error Store: " + ex.Message + " - " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException, "Error Store: " + ex.InnerException.Message);
                }
                return "Fehler beim DB Update : " + ex.Message + "\r\nStatus nur kurzfristig übernommen.";
            }
        }

        public static bool Add(Activity activity)
        {
            var key = GetKey(activity);
            if (_starter.ContainsKey(key))
            {
                Log.Debug("Ignore known user Recipient: " + activity.Recipient.Id + " From " + activity.From.Id + " " + activity.From.Name + " key="+ key);
                return false;
            }

            var conversationStarter = new NotificationManager();
            conversationStarter.Session = new BotUserSession();
            conversationStarter.Session.toId = activity.From.Id;
            conversationStarter.Session.toName = activity.From.Name;
            conversationStarter.Session.fromId = activity.Recipient.Id;
            conversationStarter.Session.fromName = activity.Recipient.Name;
            conversationStarter.Session.serviceUrl = activity.ServiceUrl;
            conversationStarter.Session.channelId = activity.ChannelId;
            conversationStarter.Session.conversationId = activity.Conversation.Id;
            conversationStarter.Session.active = true;

            _starter.Add(key, conversationStarter);
            Log.Debug("Add notify Recipt " + activity.Recipient.Id + " From " + activity.From.Id);
            try
            {
                DBAccess.SaveBotUserSessionData(conversationStarter.Session);                
            } catch (Exception ex)
            {
                Log.Error(ex, "Error Store: " + ex.Message + " - " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException,"Error Store: " + ex.InnerException.Message);
                }
            }
            return true;
        }

        private static string GetKey(IMessageActivity activity)
        {
            return activity.Recipient.Id + activity.Recipient.Name + activity.From.Name;
        }

        public static BotUserSession GetUser(Activity activity)
        {
            if (_starter.ContainsKey(activity.Recipient.Id + activity.Recipient.Name))
            {
                return null;
            }
            return _starter[activity.Recipient.Id + activity.Recipient.Name].Session;
        }

            private static CloudTable GetCloudTable()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudTableClient client = storageAccount.CreateCloudTableClient();
            CloudTable table = client.GetTableReference("botsessions");
            table.CreateIfNotExists();
            return table;
        }

        public static String GetUserAsString()
        {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append(_starter.Count).AppendLine(" Clients(s) are registered" );
            foreach(var user in _starter.Values)
            {
                AddUserAsReadAbleString(user.Session, resultBuilder);
            }
            return resultBuilder.ToString();
        }

        private static void AddUserAsReadAbleString(BotUserSession user, StringBuilder resultBuilder)
        {
            resultBuilder.Append("-- ").Append(user.toName).Append(" (").Append(user.toId).AppendLine(") --");
            resultBuilder.Append("  -> From").Append(user.fromName).Append(" (").Append(user.fromId).AppendLine(")");
            resultBuilder.Append("  -> channel ").Append(user.channelId).AppendLine();
            resultBuilder.Append("  -> active: ").Append(user.active).AppendLine();
        }

        public static async Task NotifyAsync(String text, JObject result)
        {
            LastMessage = text;
            int i = 0;
            foreach(var starter in _starter.Values)
            {
                try
                {
                    if (!starter.Session.active)
                    {
                        if (result!=null) result.Add("Ignore Notify" + i, starter.Session.toId);
                        Log.Debug("Ignore Notify" + i + " " + starter.Session.toId);
                    }

                    await starter.NotifyThisAsync(text);
                    if (result != null) result.Add("Notify"+i, starter.Session.toId);
                } catch (Exception ex)
                {
                    
                    Log.Error(ex, "Error notify: " +  ex.Message);
                    if (result != null) result.Add("Error_notify_" + i, ex.Message);
                }
                finally
                {
                    i++;
                }
            }
        }

        public async Task NotifyThisAsync(String text)
        {
            // Use the data stored previously to create the required objects.
            var userAccount = new ChannelAccount(Session.toId, Session.toName);
            var botAccount = new ChannelAccount(Session.fromId, Session.fromName);
            var connector = new ConnectorClient(new Uri(Session.serviceUrl));

            // Create a new message.
            IMessageActivity message = Activity.CreateMessageActivity();
            if (!string.IsNullOrEmpty(Session.conversationId) && !string.IsNullOrEmpty(Session.channelId))
            {
                // If conversation ID and channel ID was stored previously, use it.
                message.ChannelId = Session.channelId;
            }
            else
            {
                // Conversation ID was not stored previously, so create a conversation. 
                // Note: If the user has an existing conversation in a channel, this will likely create a new conversation window.
                Session.conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }

            // Set the address-related properties in the message and send the message.
            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: Session.conversationId);
            message.Text = text;
            message.Locale = "de-de";
            await connector.Conversations.SendToConversationAsync((Activity)message);
            Log.Debug("Notify done: " + Session.toId + ": " + Session.toName);
        }        
    }

    [Serializable]
    public class SurveyDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Hello, I'm the alert dialog. I'm interrupting your conversation to inform you that your bike is gone. Please type done.");

            context.Wait(this.MessageReceivedAsync);
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if ((await result).Text == "done")
            {
                await context.PostAsync("Great, back to the original conversation!");
                context.Done(String.Empty); // Finish this dialog.
            }
            else
            {
                await context.PostAsync("I'm still on the survey until you type \"done\"");
                context.Wait(MessageReceivedAsync); // Not done yet.
            }
        }
    }
}