using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using NotifyBotApp.Helper;
using NotifyBotApp.Models;

namespace NotifyBotApp.Controllers
{
    [Authorize]
    public class LoggingController : Controller
    {
        // GET: Logging
        public ActionResult Index()
        {
            var logFile = new LogFile();
            logFile.Lines = new List<LogEntry>();



            try
            {
                using (StreamReader sr = new StreamReader(
                    new FileStream(LoggingHelper.LocalLogFile.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd")), FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        JObject ob = JObject.Parse(line);
                        var logEntry = new LogEntry()
                        {   
                            Timestamp = ob["@t"]?.ToString(),
                            Message = ob["@mt"]?.ToString(),
                            Caller = ob["Caller"]?.ToString(),
                            Error = ob["@x"]?.ToString(),
                            Level = ob["@l"]?.ToString(),
                        };
                        logFile.Lines.Add(logEntry);
                        var matches = new Regex("\\{([a-zA-Z]+)\\}").Matches(logEntry.Message);
                        foreach (Match match in matches)
                        {
                            if (ob[match.Groups[1].Value] != null)
                            {
                                logEntry.Message = logEntry.Message.Replace(match.Value, ob[match.Groups[1].Value].ToString());
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logFile.Lines.Add(new LogEntry()
                {
                    Message = ex.Message
                });
            }

            return View(logFile);
        }
    }
}