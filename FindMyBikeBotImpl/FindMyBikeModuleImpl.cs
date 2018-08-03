using NotifyBot.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FindMyBikeBotImpl.Dialog;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;

namespace FindMyBikeBotImpl
{
    [Export(typeof(INotifyBotModule))]
    public class FindMyBikeModuleImpl : INotifyBotModule
    {
        private NotifyBotSettings _settings = new NotifyBotSettings()
        {
            BotId = "FindMyBike",
            MicrosoftAppId = ConfigurationManager.AppSettings["FindMyBike-MicrosoftAppId"],
            MicrosoftAppPassword = ConfigurationManager.AppSettings["FindMyBike-MicrosoftAppPassword"],
            MongoDbConnectionString = null
        };


        public IDialog<object> GetDefaultDialogForKnownClients()
        {
            return new RootDialog();
        }

        public IDialog<object> GetWelcomeDialogForNewClients()
        {
            return new WelcomeDialog();
        }

        public void DoUpdateBotBuilderContainer(ContainerBuilder builder, IDBAccess dbAccess)
        {
            //No costum actions
        }

        public NotifyBotSettings Settings { get => _settings; }
    }
}
