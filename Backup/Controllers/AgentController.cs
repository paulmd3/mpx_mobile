using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using MPXMobile.Models;
using System.Threading;
using System.Web.Security;

namespace MPXMobile.Controllers
{
    public class AgentController : BaseController
    {

        // GET: /Agent/
        public ActionResult Agent()
        {

            AgentInformations();
            bool aut = false;
            try
            {
                var u = Session["user"].ToString();
                var p = Session["pass"].ToString();
                Controllers.AccountController lu = new AccountController();
                aut = lu.ValidateLogOn(u, p);

            }
            catch { }

            if (Request.IsAuthenticated || aut)
            {
                var name = string.Empty;
                name = Thread.CurrentPrincipal.Identity.Name;
                if (name == "")
                    name = Session["user"].ToString();
                else
                    FormsAuthentication.SetAuthCookie(name, true);

                Models.Util.SaveLogIn(name);
                Models.Util.updateLogUser(name);
                return RedirectToAction("DefaultMenu", "Home");
            }
            else
            {
                return RedirectToAction("LogOn", "Account");
            }
        }


        public AgentInformation AgentInformations()
        {

            if (Request.Headers["User-Agent"] != null)
            {
                string DeviceType = string.Empty;
                string DeviceName = string.Empty;
                //Session.Add("Mobile",false);
                
                if (Request.Browser["IsMobileDevice"] != null && Request.Browser["IsMobileDevice"] == "true")
                {
                    Session.Add("Device", "iPhone");
                    // MOBILE DEVICE DETECTED --------------------------------------
                    DeviceType = "Mobile Device";

                    // CHECK USER AGENTS STRINGS FIRST --------------------------------

                    if (Request.UserAgent.ToLower().Contains("windows"))
                    {
                        DeviceName = "Windows Mobile";
                    }
                    else if (Request.UserAgent.ToUpper().Contains("MIDP") || Request.UserAgent.ToUpper().Contains("CLDC"))
                    {
                        DeviceName = "Other";
                    }
                    else if (Request.UserAgent.ToLower().Contains("psp") || Request.UserAgent.ToLower().Contains("playstation portable"))
                    {
                        DeviceName = "Sony PSP";
                    }


                }
                else if (Request.UserAgent.ToLower().Contains("blackberry"))
                {
                    // DOES NOT IDENTIFY ITSELF AS A MOBILE DEVICE
                    // RIM DEVICES (BlackBerry) -------------
                    DeviceType = "Mobile Device";
                    DeviceName = "BlackBerry";
                    Session.Add("Device", "iPhone");
                }
                else if (Request.UserAgent.Contains("Android"))
                {
                    // DOES NOT IDENTIFY ITSELF AS A MOBILE DEVICE
                    // RIM DEVICES (BlackBerry) -------------
                    DeviceType = "Mobile Device";
                    DeviceName = "Android";
                    Session.Add("Device", "iPhone");
                }
                else if (Request.UserAgent.Contains("iPhone"))
                {
                    // DOES NOT IDENTIFY ITSELF AS A MOBILE DEVICE

                    // IPHONE/IPOD DEVICES ------------------
                    DeviceType = "Mobile Device";
                    DeviceName = "iPhone";
                    Session.Add("Device", "iPhone");
                    
                }
                else
                {

                    // NOT A MOBILE DEVICE
                    DeviceType = "NOT a Mobile Device";
                    Session.Add("Device", "No");
                }
            }



            // SPIT OUT A BUNCH OF STUFF -----------------------------------------
            System.Web.HttpBrowserCapabilitiesBase browser = Request.Browser;

            AgentInformation aInformation = new AgentInformation();
            aInformation._Type = browser.Type;
            aInformation._Name = browser.Browser;
            aInformation._Majorversion = browser.MajorVersion.ToString();
            aInformation._MinorVersion = browser.MinorVersion.ToString();
            aInformation._Platform = browser.Platform;
            aInformation._IsBeta = browser.Beta.ToString();
            aInformation._IsCrawler = browser.Crawler.ToString();
            aInformation._IsAol = browser.AOL.ToString();
            aInformation._IsWin16 = browser.Win16.ToString();
            aInformation._IsWin32 = browser.Win32.ToString();
            aInformation._SupportFrames = browser.Frames.ToString();
            aInformation._SupportTables = browser.Tables.ToString();
            aInformation._SupportCookies = browser.Cookies.ToString();
            aInformation._SupportVBScrpt = browser.VBScript.ToString();
            aInformation._SupportJS = browser.EcmaScriptVersion.ToString();
            aInformation._SupportJavaApplets = browser.JavaApplets.ToString();
            aInformation._SupportActiveX = browser.ActiveXControls.ToString();

            // SPIT OUT THE WHOLE USER AGENT --------------------------------------------
            var DeviceAgent = Session["Device"];

            ViewData["DeviceAgent"] = DeviceAgent;
            
            ViewData["AgentInformation"] = aInformation;
            return (aInformation);
        }
    }
}
