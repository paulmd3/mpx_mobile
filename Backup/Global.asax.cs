using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Diagnostics;
using System.Web.Configuration;

namespace MPXMobile
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Agent", action = "Agent", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {

            Exception ex = Server.GetLastError();
            Exception ex2 = Server.GetLastError().GetBaseException();
            Server.ClearError();
            

            if (ex is System.Web.HttpRequestValidationException || ex2 is System.Web.HttpRequestValidationException)
            {
                Response.Redirect("ValidationError");
            }
        }
    }
}