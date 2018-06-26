using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using WhereIsMyBikeBotApp.Helper;
using WhereIsMyBikeBotApp.Models;

namespace WhereIsMyBikeBotApp.Controllers
{
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
                        logFile.Lines.Add(new LogEntry()
                        {   
                            Timestamp = ob["@t"]?.ToString(),
                            Message = ob["@mt"]?.ToString(),
                            Caller = ob["Caller"]?.ToString(),
                            Error = ob["@x"]?.ToString(),
                            Level = ob["@l"]?.ToString(),
                        });
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