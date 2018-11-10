using HealthSolution.Dal;
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
    public class MedicalFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var myController = actionContext.Controller;

            try
            {
                //Faço a verificação apenas em produção
                if (ConfigurationManager.AppSettings["SECURITY_USER"].ToString().Equals("true"))
                {
                    var userName = actionContext.HttpContext.Session["userId"].ToString();

                    //Se o usuário não tiver logado ou a sessão tiver expirado                
                    if (userName == null)
                    {
                        actionContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                    }
                    else
                    {
                        HealthContext context = null;

                        try
                        {
                            context = new HealthContext();                            
                            var currentUser = context.Usuarios.Where(x => x.Name == userName).FirstOrDefault();

                            if (currentUser != null)
                            {
                                if (!currentUser.IsMedical)
                                    actionContext.Result = new RedirectToRouteResult(
                                     new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                            }
                            else
                            {
                                actionContext.Result = new RedirectToRouteResult(
                                new RouteValueDictionary(new { controller = "Home", action = "Index" }));
                            }
                            context.Dispose();
                        }
                        catch (Exception e)
                        {
                            DebugLog.Logar(e.Message);
                            DebugLog.Logar(e.StackTrace);
                            DebugLog.Logar(Utility.Details(e));
                            if (context != null)
                                context.Dispose();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugLog.Logar(e.Message);
                DebugLog.Logar(e.StackTrace);
                DebugLog.Logar(Utility.Details(e));
                actionContext.Result = new RedirectToRouteResult(
                       new RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
            base.OnActionExecuting(actionContext);
        }
    }
}