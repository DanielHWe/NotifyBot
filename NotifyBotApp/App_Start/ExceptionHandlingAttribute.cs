using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using Serilog;
using NotifyBotApp.Models;

namespace NotifyBotApp
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            
            //Log Critical errors
            Log.Warning(context.Exception, "OnException {controller}: {message}", context.ActionContext.ControllerContext.ControllerDescriptor.ControllerName,context.Exception.Message);

            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An error occurred, please try again or contact the administrator."),
                ReasonPhrase = "Critical Exception"
            });
        }
    }
}