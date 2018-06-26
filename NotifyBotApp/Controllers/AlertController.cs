using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Routing;
using WhereIsMyBikeBotApp.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using Serilog;

namespace WhereIsMyBikeBotApp.Controllers
{
    public class AlertController: ApiController
    {
        [HttpPost]
        [Route("api/alert")]
        public async Task<JObject> Post(string text)
        {
            try
            {
                var result = new JObject();

                await NotificationManager.NotifyAsync(text, result);

                /*foreach (QueueMessage conversationReference in RootDialog.ResumptionCookieList)
                {
                    if (conversationReference == null)
                    {
                        result.Add("Error", "conversationReference is empty - ignore this conversationReference");
                        Log.Add("conversationReference is empty - ignore this conversationReference");
                        continue;
                    }

                    //var message = conversationReference.GetPostToBotMessage();

                    
                    ConnectorClient client = new ConnectorClient(new Uri(message.ServiceUrl));

                    try
                    {
                        var conversation = await client.Conversations.CreateDirectConversationAsync(message.Recipient, message.From);
                        var message = conversation.;
                        message.Conversation.Id = conversation.Id;

                        var reply = message.CreateReply();
                        reply.Text = text;
                        result.Add("Send", text);
                        result.Add("To", message.Recipient.Name);

                        await client.Conversations.SendToConversationAsync(reply);
                    }
                    catch (Exception ex)
                    {
                        var reply = message.CreateReply();
                        reply.Text = ex.Message;
                        result.Add("Error", ex.Message);

                        await client.Conversations.SendToConversationAsync(reply);

                        continue;
                    }
                }*/

                
               
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex,String.Format("Error '{0} on Messages: {1} - \r\n{2}", ex.GetType().Name, ex.Message, ex.StackTrace));
                var result = new JObject();
                result.Add("Error", ex.GetType().Name);
                result.Add("Messages", ex.Message);
                result.Add("StackTrace", ex.StackTrace);

                return result;
            }
        }
    }
}