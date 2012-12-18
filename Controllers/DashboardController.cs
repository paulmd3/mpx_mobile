using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MPXMobile.Controllers
{
    [HandleError]
    public class DashboardController : BaseController
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                return View("_Index");
            }
            return View();
        }
    }
}
