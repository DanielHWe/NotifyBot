using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhereIsMyBikeBotApp.Models
{
    public class LogEntry
    {
        public String Timestamp { get; set; }
        public String Message { get; set; }
        public string Caller { get; set; }
        public string Error { get; set; }
        public string Level { get; set; }
    }
}