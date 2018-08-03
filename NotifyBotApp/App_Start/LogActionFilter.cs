using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Routing;
using NotifyBotApp.Models;
using Exception = System.Exception;

namespace NotifyBotApp.App_Start
{
    public class LogActionFilter : ActionFilterAttribute

    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            if (actionExecutedContext.ActionContext != null && actionExecutedContext.ActionContext.Response != null)
            {
                LogResponseCode(actionExecutedContext);
            }

            if (actionExecutedContext.Exception != null)
            {
                Log.Error("Error: " + actionExecutedContext.Exception.Message);
            }
           
            base.OnActionExecuted(actionExecutedContext);
        }

        private static void LogResponseCode(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                if (actionExecutedContext.ActionContext.Response.StatusCode >= HttpStatusCode.Forbidden)
                {
                    Log.Warning(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " executed on "
                                                                                                + actionExecutedContext
                                                                                                    .ActionContext
                                                                                                    .ControllerContext
                                                                                                    .ControllerDescriptor
                                                                                                    .ControllerName +
                                                                                                "," +
                                                                                                actionExecutedContext
                                                                                                    .ActionContext
                                                                                                    .Response
                                                                                                    .StatusCode);
                }
                else if (actionExecutedContext.ActionContext.Response.StatusCode >= HttpStatusCode.BadRequest)
                {
                    Log.Warning(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " executed on "
                                                                                                + actionExecutedContext
                                                                                                    .ActionContext
                                                                                                    .ControllerContext
                                                                                                    .ControllerDescriptor
                                                                                                    .ControllerName +
                                                                                                "," +
                                                                                                actionExecutedContext
                                                                                                    .ActionContext
                                                                                                    .Response
                                                                                                    .StatusCode);
                }
                else
                {
                    Log.Debug(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + " executed on "
                                                                                              + actionExecutedContext
                                                                                                  .ActionContext
                                                                                                  .ControllerContext
                                                                                                  .ControllerDescriptor
                                                                                                  .ControllerName +
                                                                                              "," +
                                                                                              actionExecutedContext
                                                                                                  .ActionContext
                                                                                                  .Response
                                                                                                  .StatusCode);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error log ResponseCode: " + ex.Message);
            }
        }
    }

}