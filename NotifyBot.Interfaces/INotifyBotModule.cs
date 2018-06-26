using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace NotifyBot.Interfaces
{
    public interface INotifyBotModule
    {
        IDialog<object> GetDefaultDialogForKnownClients();

        IDialog<object> GetWelcomeDialogForNewClients();

        NotifyBotSettings Settings { get; }
    }
}
