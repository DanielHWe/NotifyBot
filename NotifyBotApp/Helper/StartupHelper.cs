using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using NotifyBot.Interfaces;
using Serilog;
using WhereIsMyBikeBotApp.DataAccess;

namespace WhereIsMyBikeBotApp.Helper
{
    internal class StartupHelper
    {
        [ImportMany(typeof(INotifyBotModule))]
        INotifyBotModule[] _modules;

        internal void PerformStartUp()
        {
            try
            {
                var module = GetModule();

                ConfigurationManager.AppSettings.Set("MicrosoftAppId", module.Settings.MicrosoftAppId);
                ConfigurationManager.AppSettings.Set("MicrosoftAppPassword", module.Settings.MicrosoftAppPassword);
                ConfigurationManager.AppSettings.Set("BotId", module.Settings.BotId);

                DBAccess.ConnectionString = module.Settings.MongoDbConnectionString;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to startup Notify Bot: {type} {message}", ex.GetType().Name, ex.Message);
            }
        }

        private INotifyBotModule GetModule()
        {
            var catalog = new DirectoryCatalog(".\\bin", "*Bot*");
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            if (_modules == null || _modules.Length == 0) throw new OperationCanceledException("No implementation of INotifyBotModule was found in " + catalog.FullPath + ".");

            return _modules[0];
        }
    }
}