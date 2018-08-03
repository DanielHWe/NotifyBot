using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotifyBotApp.Models
{
    public class AlertMessageResult
    {
        public IList<AlertMessageResultItem> Items { get; private set; } = new List<AlertMessageResultItem>();

        internal void Add(string name, string value)
        {
            Items.Add(new AlertMessageResultItem()
            {
                Name = name,
                Value = value
            });
        }
    }

    public class AlertMessageResultItem
    {
        public String Name { get; set; }
        public String Value { get; set; }
    }
}