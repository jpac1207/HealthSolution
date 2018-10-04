using HealthSolution.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HealthSolution.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var myController = actionContext.Controller;

            try
            {
                //Faço a verificação apenas em produção
                if (ConfigurationManager.AppSettings["SECURITY_USER"].ToString().Equals("true"))
                {
                    var userId = actionContext.HttpContext.Session["userId"];

                    //Se o usuário não tiver logado ou a sessão tiver expirado                
                    if (userId == null)
                    {
                        actionContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
                DebugLog.Logar(Utility.Details(e));
                actionContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
            }
            base.OnActionExecuting(actionContext);
        }
    }
}