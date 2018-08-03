using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Serilog;

namespace FindMyBikeBotImpl.Dialog
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private String[] _messages = new[] { "Ich werde dein Rad finden", "Keep on riding", "Schwing dich auf den Sattel" };

        public Task StartAsync(IDialogContext context)
        {
            Log.Debug("RootDialog - StartAsync");
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Log.Debug("RootDialog - MessageReceivedAsync");
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            try
            {
                Random r = new Random();
                await context.PostAsync(_messages[r.Next(_messages.Length)]);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                await context.PostAsync("Wenn es einer klaut sag ich es dir");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}
