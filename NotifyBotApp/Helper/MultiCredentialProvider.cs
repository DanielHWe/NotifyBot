using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Connector;
using Serilog;

namespace NotifyBotApp.Helper
{
    /// <summary>
    /// A sample ICredentialProvider that is configured by multiple MicrosoftAppIds and MicrosoftAppPasswords
    /// </summary>
    public class MultiCredentialProvider : ICredentialProvider
    {
        
        public Task<bool> IsValidAppIdAsync(string appId)
        {
            var res = StartupHelper.Module.Settings.MicrosoftAppId.Equals(appId);
            if (!res) Log.Warning("Unknown AppId: " + appId.Remove(2) + "*");
            return Task.FromResult(res);
        }

        public Task<string> GetAppPasswordAsync(string appId)
        {
            var res = StartupHelper.Module.Settings.MicrosoftAppId.Equals(appId);
            if (!res) Log.Warning("Unknown AppId: " + appId.Remove(2) + "*");
            return Task.FromResult(res ? StartupHelper.Module.Settings.MicrosoftAppPassword : null);
        }

        public Task<bool> IsAuthenticationDisabledAsync()
        {
            return Task.FromResult(false);
        }
    }
}