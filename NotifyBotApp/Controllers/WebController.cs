using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using NotifyBotApp.Helper;
using NotifyBotApp.Models;

namespace NotifyBotApp.Controllers
{
    public class WebController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                return View(new AlertMessage());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error '{0} on Invite Index Get: {message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(AlertMessage alert)
        {
            try
            {
                if (ModelState.IsValid)
                {                     

                    var result = await NotificationManager.NotifyAsync(alert.Message);

                    return View("AlertResult", result);
                }
                else
                {
                    Log.Warning("Invite: Wrong information, validation failed, recall Invite page");
                    return View(alert);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error '{0} on Invite Index Post: {message}", ex.GetType().Name, ex.Message);
                throw;
            }
        }


        // GET: Logging
        public ActionResult Clients()
        {

            return View(NotificationManager.GetAllUsers());
        }
    }
}