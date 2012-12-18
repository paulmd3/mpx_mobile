using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MPXMobile.Controllers
{
    public class ErrorsController : Controller
    {
        //
        // GET: /Errors/

        [HttpGet]
        public ActionResult Http404(string source)
        {
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult Http500(string source)
        {
            Response.StatusCode = 500;

            ViewData["error"] = Server.GetLastError();

            return View();
        }

    }
}
