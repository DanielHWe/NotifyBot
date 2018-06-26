using Serilog;
using System;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Routing;
using WhereIsMyBikeBotApp.Models;

namespace WhereIsMyBikeBotApp.App_Start
{
    public class LogActionFilter : ActionFilterAttribute

    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            Log.Debug(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " executed on " 
                +actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName + "," +
                actionExecutedContext.ActionContext.Response.StatusCode );
            if (actionExecutedContext.Exception != null)
            {
                Log.Error("Error: " + actionExecutedContext.Exception.Message);
            }
           
            base.OnActionExecuted(actionExecutedContext);
        }
        

        


    }

}