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
    public class WelcomeDialog : IDialog<object>
    {


        public Task StartAsync(IDialogContext context)
        {
            Log.Debug("WelcomeDialog - StartAsync");
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            Log.Debug("WelcomeDialog - MessageReceivedAsync");
            var activity = await result as Activity;


            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            try
            {
                Random r = new Random();
                await context.PostAsync(
                    "Wilkommen beim FindMyBike Bot :-) !\r\n" +
                    "\r\n" +
                    "use \r\n" +
                    "  stop to stop notify\r\n" +
                    "  start to restart notify");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                await context.PostAsync(ex.Message);
                await context.PostAsync("Sorry da hätte nicht passieren sollten");

                context.Wait(MessageReceivedAsync);
            }
        }
    }
}
