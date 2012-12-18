using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MPXMobile.Controllers
{
    public class AppCacheController : Controller
    {
        //
        // GET: /AppCache/
        public ActionResult Manifest()
        {
            return View();
        }

    }
}
