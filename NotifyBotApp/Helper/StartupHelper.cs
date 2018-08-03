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
using NotifyBotApp.DataAccess;

namespace NotifyBotApp.Helper
{
    internal class StartupHelper
    {
        [ImportMany(typeof(INotifyBotModule))]
        INotifyBotModule[] _modules;

        public static INotifyBotModule Module { get; private set; }
        public static IDBAccess DbAccess { get; private set; }

        internal void PerformStartUp()
        {
            try
            {
                Log.Debug("Startup startet");
                Module = GetModule();
                Log.Debug("Startup for " + Module.GetType().Name);

                ConfigurationManager.AppSettings.Set("MicrosoftAppId", Module.Settings.MicrosoftAppId);
                ConfigurationManager.AppSettings.Set("MicrosoftAppPassword", Module.Settings.MicrosoftAppPassword);
                ConfigurationManager.AppSettings.Set("BotId", Module.Settings.BotId);

                StartupHelper.DbAccess = new DBAccess();
                StartupHelper.DbAccess.ConnectionString = Module.Settings.MongoDbConnectionString;
                Log.Debug("Startup done - Settings online for " + Module.GetType().Name);

                if (!ConfigurationManager.AppSettings.Get("MicrosoftAppId").Equals(Module.Settings.MicrosoftAppId))
                {
                    throw new Exception("AppSettings not saved - startup failed");
                }
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

            var defaultBotId = ConfigurationManager.AppSettings["DefaultBotId"];
            if (!String.IsNullOrEmpty(defaultBotId))
            {
                foreach (var module in _modules)
                {
                    if (module.Settings.BotId.Equals(defaultBotId))
                    {
                        return module;
                    }
                }
            }

            return _modules[0];
        }
    }
}