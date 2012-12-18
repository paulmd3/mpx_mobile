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

        void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                //
            }

            Response.Clear();
            Server.ClearError();

            string path = Request.Path;
            Context.RewritePath(string.Format("~/Errors/Http{0}", code), false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(Context);
            Context.RewritePath(path, false);
        }
    }
}