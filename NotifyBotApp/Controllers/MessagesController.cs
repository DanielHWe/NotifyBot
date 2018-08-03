
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using NotifyBotApp.Models;
using NotifyBot.Interfaces;
using Serilog;
using NotifyBotApp.Helper;


namespace NotifyBotApp
{
    [BotAuthentication(CredentialProviderType = typeof(MultiCredentialProvider))]
    public class MessagesController : ApiController
    {
        public static String CurrentClientId { get; set; }        


        public MessagesController()
        {
            Log.Debug("MessagesController created");
            AddCommandMessages();
        }

        

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity, CancellationToken token)
        {
            try
            {
                Log.Debug("Debug Messages: {type}: {text} from {id}", activity.Type, activity.Text, activity.From.Id);

                if (activity.Type == ActivityTypes.Message)
                {
                    IDialog<object> dialog;
                    if (NotificationManager.Add(activity))
                    {
                        dialog = StartupHelper.Module.GetWelcomeDialogForNewClients();
                    }
                    else
                    {
                        dialog = StartupHelper.Module.GetDefaultDialogForKnownClients();
                    }
                    Log.Debug("Execute Bot dialog {type}", dialog.GetType().Name);
                    await Conversation.SendAsync(activity, () => dialog);
                }
                else
                {
                    HandleSystemMessageAsync(activity);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);
                return response;
            }catch (Exception ex)
            {
                Log.Error(ex, "Error '{type}' on Messages: {message} - \r\n {trace}", ex.GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                IConversationUpdateActivity update = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    var client = scope.Resolve<IConnectorClient>();
                    if (update.MembersAdded.Any())
                    {
                        var reply = message.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != message.Recipient.Id)
                            {
                                reply.Text = $"Welcome {newMember.Name}!";
                            }
                            else
                            {
                                reply.Text = $"Welcome {message.From.Name}";
                            }
                            await client.Conversations.ReplyToActivityAsync(reply);
                        }
                    }
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        #region Command Messages

        private static void AddCommandMessages()
        {
            try
            {
                Conversation.UpdateContainer(builder =>
                {
                    AddCommadMessage(builder, 
                        new Regex("marco"), 
                        async (IBotToUser botToUser, IMessageActivity message) => { await botToUser.PostAsync("polo"); });

                    AddCommadMessage(builder,
                        new Regex("^(users|clients)$", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            await botToUser.PostAsync(NotificationManager.GetUserAsString());
                        });

                    AddCommadMessage(builder,
                        new Regex("^(user|client)$", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            var userText = NotificationManager.GetUserAsString(message);
                            await botToUser.PostAsync(userText);
                        });

                    AddCommadMessage(builder,
                        new Regex("^start$", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            var userText = NotificationManager.SetUserActive(message, true);
                            await botToUser.PostAsync(userText);
                        });

                    AddCommadMessage(builder,
                        new Regex("^last$", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            try
                            {
                                await NotificationManager.NotifyAsync(NotificationManager.LastMessage);
                                await botToUser.PostAsync("done");
                            }
                            catch (Exception ex)
                            {
                                await botToUser.PostAsync("FAILED: " + ex.Message);
                            }
                        });

                    AddCommadMessage(builder,
                        new Regex("^notify", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            try
                            {
                                await NotificationManager.NotifyAsync(message.Text.Substring(7));
                                await botToUser.PostAsync("done");
                            }
                            catch (Exception ex)
                            {
                                await botToUser.PostAsync("FAILED: " + ex.Message);
                            }
                        });

                    AddCommadMessage(builder,
                        new Regex("^stop$", RegexOptions.IgnoreCase),
                        async (IBotToUser botToUser, IMessageActivity message) =>
                        {
                            var userText = NotificationManager.SetUserActive(message, false);
                            await botToUser.PostAsync(userText);
                        });

                    try
                    {
                        StartupHelper.Module?.DoUpdateBotBuilderContainer(builder, StartupHelper.DbAccess);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error {type} during {bot}.DoUpdateBotBuilderContainer: {message}", ex.GetType().Name, StartupHelper.Module?.GetType().Name, ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error '{type}' on Messages: {message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        private static void AddCommadMessage(ContainerBuilder builder, Regex regex, Func<IBotToUser, IMessageActivity, Task> action)
        {
            var scorable = Actions
                .Bind(action)
                .When(regex)
                .Normalize();

            builder.RegisterInstance(scorable).AsImplementedInterfaces().SingleInstance();
        }

        #endregion
    }

    public class CustomAuthentication : BotAuthentication

    {

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)

        {
            if (StartupHelper.Module!=null)
            {
                this.MicrosoftAppId = StartupHelper.Module.Settings.MicrosoftAppId;
                this.MicrosoftAppPassword = StartupHelper.Module.Settings.MicrosoftAppPassword;
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);

        }

    }
}