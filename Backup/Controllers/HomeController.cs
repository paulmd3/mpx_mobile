using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.Caching;
using System.Threading;
using MPXMobile.Models;
using System.Xml;
using System.Web.Script.Serialization;

namespace MPXMobile.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {

        public ActionResult Manifest()
        {
            return View();
        }


        public ActionResult Offline()
        {
            return View();
        }


        public ActionResult Search()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }

            else
            {
                //ViewData["Message"] = "Welcome to MPX Mobile!";

                if (Request.IsAjaxRequest())
                {
                    return View("_Search");
                }
                return View();
            }
        }


        public ActionResult InsertNotesInformation()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {

                ///Verify if the xml has been updated
                System.Xml.XmlTextReader xtr = new System.Xml.XmlTextReader(Server.MapPath("/NoteTypes.xml"));
                xtr.WhitespaceHandling = System.Xml.WhitespaceHandling.None;
                System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
                xDoc.Load(xtr);
                var date = xDoc.SelectSingleNode("// types/date[1]/modified").InnerText;

                if (DateTime.Parse(date).AddMonths(int.Parse(System.Configuration.ConfigurationSettings.AppSettings["NoteTypes"])).ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    Session.Add("createXML", "true");
                }

                return View();
            }
        }


        public ActionResult About()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                return View();
            }
        }


        public ActionResult SearchDonors(string keyword)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    ViewData["donors"] = Models.Donors.SearchDonors(keyword);

                    if (Request.IsAjaxRequest())
                    {
                        return View("_SearchDonors");
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }

        
        public ActionResult DefaultMenu()
        {
            if (Session["createXML"] != null)
            {
                create_XML();
                Session.Remove("createXML");
            }
            return View("Defaultmenu");

        }

        
        public ActionResult BookmarkedDonors()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                try
                {
                    var list = Models.Donors.BookmarkedByUser(HttpContext.User.Identity.Name);
                    ViewData["BookmarkedDonors"] = list;

                    if (Request.IsAjaxRequest())
                    {
                        return View("_BookMarked");
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                    return View("Error");
                }
            }
        }


        public ActionResult BookmarkDonor(int accountId)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                Models.BookmarkedDonor book = new MPXMobile.Models.BookmarkedDonor();
                book.Donor_ID = accountId;
                book.User_ID = HttpContext.User.Identity.Name;
                var status = Models.Donors.BookmarkDonor(book);
                ViewData["BookmarkedDonors"] = status;

                if (IsAjaxRequest())
                {
                    string message = string.Empty;
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    object jsonData = serializer.DeserializeObject(status.ToString());
                    return Json(new { jsonData });

                }
                else
                {
                    return RedirectToAction("BookmarkedDonors", "Home");
                }
            }
        }


        public ActionResult RecentlyDonors()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }

            else
            {
                try
                {
                    //list of recently donors saved in cache
                    var list = HttpRuntime.Cache.Get("listaccount");
                    Models.Util.SaveAction(Models.Util.Actions.GetRecentlyDonors.ToString());
                    ViewData["RecentlyDonors"] = list;

                    if (Request.IsAjaxRequest())
                    {
                        return View("_Recently");
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                    return View("Error");
                }
            }
        }


        public ActionResult ValidationError()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                return View();
            }
        }


        public ActionResult DonorInformation(string donorId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    var donor = Models.Donors.GetAccount(donorId);
                    var loadRecentlyDonors = Models.Donors.Recently_Donors(donor);
                    Models.Util.SaveAction(Util.Actions.GetDonorInformation.ToString());
                    ViewData["account"] = donor;

                    if (Request.IsAjaxRequest())
                    {
                        return View("_Donor");
                    }
                    return View();
                }

            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }


        public ActionResult GiftInformation(string accountId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    var gift = Models.Gifts.GetGiftSummaries(accountId);
                    ViewData["account"] = gift;
                    if (Request.IsAjaxRequest())
                    {
                        return View("_Gift");
                    }
                    return View();
                }


            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }


        public ActionResult NotesInformation(string accountId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    var notes = Models.Notes.GetAccountNotes(accountId);

                    Models.Util.SaveAction("Get Gift Notes donor: " + accountId);
                    ViewData["notes"] = notes;
                    if (Request.IsAjaxRequest())
                    {
                        return View("_Notes");
                    }
                    return View();
                }

            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }


        /// <summary>
        /// Create the xml file with the Note Types using  file NoteTypes.xml
        /// </summary>
        public void create_XML()
        {

            // location to the XML file to write
            String strFilePath = (Server.MapPath("/NoteTypes.xml"));

            // create an instance of the XmlTextWriter object
            XmlTextWriter objWriter = new XmlTextWriter(strFilePath, System.Text.Encoding.UTF8);

            // start writing the XML document
            objWriter.WriteStartDocument();

            // write a comment in our XML file
            objWriter.WriteComment("MPX Note Types");

            objWriter.WriteStartElement("types");

            objWriter.WriteStartElement("date", null);

            objWriter.WriteElementString("modified", DateTime.Now.ToShortDateString());

            // close  element
            objWriter.WriteEndElement();


            foreach (var item in Models.Notes.GetTypes().noteTypes)
            {

                // output the first element
                objWriter.WriteStartElement("type", null);

                objWriter.WriteElementString("noteTypeValue", item.noteTypeValue);
                objWriter.WriteElementString("description", item.description);

                // close  element
                objWriter.WriteEndElement();
            }

            // flush and write XML data to the file
            objWriter.Flush();

            // clear up memory
            objWriter.Close();

        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertNotesInformation(string type, string note, string donorId,string noteId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    if (noteId != null)
                    {
                        EditNotesInformation(int.Parse(noteId), type, note, donorId);
                        return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                    }
                    Models.Notes.InsertNotes(Session["donorNotes"].ToString(), type, note);
                    Models.Util.SaveAction("Insert Note: " + Session["donorNotes"]);
                    return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                }

            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditNotesInformation(int noteId, string type, string note, string donorId)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else
                {
                    Models.Notes.UpdateNotes(Session["donorNotes"].ToString(), type, note,noteId);
                    Models.Util.SaveAction("Update Note: " + Session["donorNotes"]);
                    return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                }

            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                return View("Error");
            }
        }

        public ActionResult DeleteNote(string noteId)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                try
                {
                    Models.Notes.DeleteNotes(int.Parse(noteId));
                    Models.Util.SaveAction("Delete Note: " + Session["donorNotes"]);
                    return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                    return View("Error");
                }
            }
        }


    }
}
