using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotifyBotApp.Models
{
    public class AlertMessage
    {
        [Required]
        [Display(Name = "Message")]
        public String Message { get; set; }
    }
}