using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using Serilog;
using Serilog.Formatting.Compact;

namespace WhereIsMyBikeBotApp.Helper
{
    public static class LoggingHelper
    {
        public static readonly string LocalLogDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Kanban", "log");

        public static readonly string LocalLogFile = Path.Combine(LocalLogDirectory, "log-{Date}.json");

        /// <summary>
        /// Initialize Logging
        /// </summary>
        /// <remarks>
        /// We log in different targets:
        ///     File
        ///     Console
        ///     Fluentd - that is the main target. It is forwarded to the Haufe Group central looging instance
        ///     AzureTableStorage - uses hgserviceslogging storageaccount
        /// </remarks>
        public static void InitLogger()
        {
            Log.Logger = GetLogger(GetLoggerConfig());
            Log.Logger.Here(typeof(LoggingHelper)).Information("Logging initialized");
        }

        private static LoggerConfiguration GetLoggerConfig()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteToRollingFile();
        }

        private static LoggerConfiguration WriteToRollingFile(this LoggerConfiguration loggerConfig)
        {
            return loggerConfig.WriteTo.RollingFile(new CompactJsonFormatter(), LocalLogFile);
        }

        private static ILogger GetLogger(LoggerConfiguration loggerConfig)
        {
            return loggerConfig.CreateLogger().ForContext("Service", "Gamification");
        }

        /// <summary>
        /// Add context information about calling type and member to logger
        /// </summary>
        /// <param name="logger">Logger that gets the calling information</param>
        /// <param name="caller">Caller</param>
        /// <param name="callerMemberName">Calling member name</param>
        public static ILogger Here<T>(this ILogger logger, T caller, [CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return logger.Here(typeof(T), callerMemberName);
        }

        /// <summary>
        /// Add context information about calling type and member to logger
        /// </summary>
        /// <param name="logger">Logger that gets the calling information</param>
        /// <param name="callingType">Calling type</param>
        /// <param name="callerMemberName">Calling member name</param>
        public static ILogger Here(this ILogger logger, Type callingType, [CallerMemberName] string callerMemberName = "")
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            return logger
                .ForContext(Serilog.Core.Constants.SourceContextPropertyName, callingType.FullName)
                .ForContext("Caller", callerMemberName);
        }
    }
}