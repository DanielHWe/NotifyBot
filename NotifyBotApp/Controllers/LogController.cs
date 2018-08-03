using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NotifyBotApp.Models;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using NotifyBotApp.Helper;

namespace NotifyBotApp.Controllers
{
    public class LogController : ApiController
    {
        

        [HttpGet]
        [Route("api/log/all")]
        public JObject GetAll()
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
                            Message = line
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

            var result = new JObject();
            var gamesArray = JArray.FromObject(logFile.Lines.ToArray());
            result.Add("log", gamesArray);

            return result;
        }
    }
}