using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;

namespace NotifyBot.Interfaces
{
    public interface INotifyBotModule
    {
        IDialog<object> GetDefaultDialogForKnownClients();

        IDialog<object> GetWelcomeDialogForNewClients();

        void DoUpdateBotBuilderContainer(ContainerBuilder builder, IDBAccess dbAccess);

        NotifyBotSettings Settings { get; }
    }
}
