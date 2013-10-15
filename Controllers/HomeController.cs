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
using MPXMobile.WipService;
using System.Collections;
using System.Globalization;

namespace MPXMobile.Controllers
{
    [HandleError]
    public class HomeController : BaseController
    {
        public IUseApiDonors Service { private get; set; }
        public IUseApiGifts GiftService { private get; set; }

        public HomeController()
        {
            Service = new ApiServiceDonors();
            GiftService = new ApiServiceGifts();
        }

        public ActionResult Manifest()
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

                if (DateTime.Parse(date).AddMonths(int.Parse(System.Configuration.ConfigurationManager.AppSettings["NoteTypes"])).ToShortDateString() == DateTime.Now.ToShortDateString())
                {
                    Session.Add("createXML", "true");
                }

                if (Request.IsAjaxRequest())
                {
                    return View("_InsertNotes");
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

        public ActionResult SearchDonors(string email, string account, string org, string name, string zip)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("LogOn", "Account");
                }
                else if (email == null || account == null || org == null || name == null || zip == null)
                {
                    return RedirectToAction("Search", "Home");
                }

                SearchCondition condition = new SearchCondition()
                {
                    Email = email.Trim(),
                    AccountNumber = account.Trim(),
                    Organization = org.Trim(),
                    Name = name.Trim(),
                    Zip = zip.Trim()
                };
                List<account> list = Service.Search(condition);

                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                //Create TextInfo object.
                TextInfo textInfo = cultureInfo.TextInfo;

                foreach (account acc in list)
                {
                    acc.lastName = textInfo.ToTitleCase(acc.lastName.Trim());
                    acc.firstName = textInfo.ToTitleCase(acc.firstName.Trim());
                }

                IEnumerable<account> accounts;

                if (org.Length > 0)
                {
                    accounts = from a in list where a.isOrganization == true orderby a.lastName, a.firstName select a;
                }
                else
                {
                    accounts = from l in list orderby l.lastName, l.firstName select l;
                }
                //list = list.OrderBy(l => l.lastName).ThenBy(l => l.lastName).ToList();
                //list = list.OrderBy(l => l.accountId).ToList();

                SortedDictionary<string, List<account>> dic = new SortedDictionary<string, List<account>>();
                char letter = 'A';

                foreach (account acc in accounts)
                {
                    if (acc.lastName != "")
                    {
                        letter = acc.lastName[0];

                        if (dic.ContainsKey(letter.ToString().ToUpper()))
                        {
                            dic[letter.ToString().ToUpper()].Add(acc);
                        }
                        else
                        {
                            List<account> accs = new List<account>();
                            accs.Add(acc);
                            dic.Add(letter.ToString().ToUpper(), accs);
                        }
                    }
                }

                if (Request.IsAjaxRequest())
                {
                    return View("_SearchDonors", dic);
                }
                return View(dic);
            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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

        public ActionResult BookmarkedDonors(string donorId)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(donorId))
                    {
                        ViewData["donorId"] = donorId;
                    }
                    var list = Service.BookmarkedByUser(HttpContext.User.Identity.Name);

                    List<account> listBooked = new List<account>();

                    foreach (var item in list)
                    {
                        listBooked.Add(Service.GetAccount(Convert.ToInt64(item.Donor_ID)));
                    }

                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //Create TextInfo object.
                    TextInfo textInfo = cultureInfo.TextInfo;

                    foreach (account acc in listBooked)
                    {
                        acc.lastName = textInfo.ToTitleCase(acc.lastName.Trim());
                        acc.firstName = textInfo.ToTitleCase(acc.firstName.Trim());
                    }
                    var accounts = from l in listBooked orderby l.lastName, l.firstName select l;

                    SortedDictionary<string, List<account>> dic = new SortedDictionary<string, List<account>>();
                    char letter = 'A';

                    foreach (account acc in accounts)
                    {
                        if (acc.lastName != "")
                        {
                            letter = acc.lastName[0];

                            if (dic.ContainsKey(letter.ToString().ToUpper()))
                            {
                                dic[letter.ToString().ToUpper()].Add(acc);
                            }
                            else
                            {
                                List<account> accs = new List<account>();
                                accs.Add(acc);
                                dic.Add(letter.ToString().ToUpper(), accs);
                            }
                        }
                    }

                    if (Request.IsAjaxRequest())
                    {
                        return View("_BookMarked", dic);
                    }

                    return View(dic);
                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
                var status = Service.BookmarkDonor(book);
                ViewData["BookmarkedDonors"] = status;

                //Changed on 20130209, let only redirct to bookmark page
                //if (IsAjaxRequest())
                //{
                //    string message = string.Empty;
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    object jsonData = serializer.DeserializeObject(status.ToString());
                //    return Json(new { jsonData });

                //}
                //else
                //{
                //return RedirectToAction("BookmarkedDonors?donorId=" + accountId, "Home");
                return RedirectToAction("DonorInformation?donorId=" + accountId, "Home");
                //}
            }
        }

        public ActionResult UnBookmarkDonor(int accountId)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("LogOn", "Account");
            }
            else
            {
                var status = (new ApiServiceDonors()).UnBookmarkDonor(HttpContext.User.Identity.Name, accountId);
                ViewData["BookmarkedDonors"] = status;

                //Changed on 20130209, let only redirct to bookmark page
                //if (IsAjaxRequest())
                //{
                //    string message = string.Empty;
                //    JavaScriptSerializer serializer = new JavaScriptSerializer();
                //    object jsonData = serializer.DeserializeObject(status.ToString());
                //    return Json(new { jsonData });

                //}
                //else
                //{
                return RedirectToAction("DonorInformation?donorId=" + accountId, "Home");
                //}
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
                    if (HttpRuntime.Cache.Get("listaccount") == null)
                    {
                        if (Request.IsAjaxRequest())
                        {
                            return View("_Recently");
                        }

                        return View();
                    }
                    //list of recently donors saved in cache
                    List<account> list = (List<account>)HttpRuntime.Cache.Get("listaccount");
                    Models.Util.SaveAction(Models.Util.Actions.GetRecentlyDonors.ToString());

                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                    //Create TextInfo object.
                    TextInfo textInfo = cultureInfo.TextInfo;

                    foreach (account acc in list)
                    {
                        acc.lastName = textInfo.ToTitleCase(acc.lastName.Trim());
                        acc.firstName = textInfo.ToTitleCase(acc.firstName.Trim());
                    }
                    var accounts = from l in list orderby l.lastName, l.firstName select l;
                    //list = list.OrderBy(l => l.lastName).ThenBy(l => l.lastName).ToList();
                    //list = list.OrderBy(l => l.accountId).ToList();

                    SortedDictionary<string, List<account>> dic = new SortedDictionary<string, List<account>>();
                    char letter = 'A';

                    foreach (account acc in accounts)
                    {
                        if (acc.lastName != "")
                        {
                            letter = acc.lastName[0];

                            if (dic.ContainsKey(letter.ToString().ToUpper()))
                            {
                                dic[letter.ToString().ToUpper()].Add(acc);
                            }
                            else
                            {
                                List<account> accs = new List<account>();
                                accs.Add(acc);
                                dic.Add(letter.ToString().ToUpper(), accs);
                            }
                        }
                    }
                    ViewData["RecentlyDonors"] = dic;
                    if (Request.IsAjaxRequest())
                    {
                        return View("_Recently");
                    }

                    return View();
                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
                    long id = Convert.ToInt64(donorId);

                    account donor = Service.GetAccount(id);
                    var loadRecentlyDonors = Service.RecentlyDonors(donor);
                    Models.Util.SaveAction(Util.Actions.GetDonorInformation.ToString());

                    DonorDetail donorDetail = new DonorDetail() { Donor = donor };
                    donorDetail.Addresses = Service.GetAccountAddress(id).OrderBy(a => a.state).ThenBy(a => a.city).ThenBy(a => a.address1).ToList();
                    donorDetail.Emails = Service.GetAccountEmail(id).OrderBy(e => e.emailAddress).ToList();
                    donorDetail.Phones = Service.GetAccountPhone(id).OrderBy(p => p.phoneNumber).ToList();

                    List<BookmarkedDonor> list = (new ApiServiceDonors()).GetBookmarkedByUser(HttpContext.User.Identity.Name, Int32.Parse(donorId));
                    if (list != null && list.Count > 0)
                    {
                        ViewData["IsBookmarked"] = true;
                    }
                    else
                    {
                        ViewData["IsBookmarked"] = false;
                    }

                    if (Request.IsAjaxRequest())
                    {
                        return View("_Donor", donorDetail);
                    }
                    return View(donorDetail);
                }
            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
                    GiftInfo gift = new GiftInfo()
                    {
                        Donor = Service.GetAccount(Convert.ToInt64(accountId)),
                        Gifts = GiftService.GetGift(accountId)
                    };

                    if (Request.IsAjaxRequest())
                    {
                        return View("_Gift", gift);
                    }
                    return View(gift);

                    //var gift = GiftService.GetGiftSummaries(Convert.ToInt64(accountId));

                    //List<giftSummary> list = new List<giftSummary>();

                    //foreach (giftSummary item in gift.giftSummaries)
                    //{
                    //    if (!item.firstGiftDate.ToShortDateString().Equals("1/1/0001"))
                    //    {
                    //        list.Add(item);
                    //    }
                    //}
                    //var sum = GiftService.GetGiftAnnualSummaries(Convert.ToInt64(accountId));
                    //ViewData["account"] = list;
                    //if (Request.IsAjaxRequest())
                    //{
                    //    return View("_Gift");
                    //}
                    //return View();
                }
            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
                    Session.Add("donorNotes", accountId);
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
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
        public ActionResult InsertNotesInformation(string type, string note, string noteId)
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
                        Models.Notes.UpdateNotes(Session["donorNotes"].ToString(), type, note, int.Parse(noteId));
                        Models.Util.SaveAction("Update Note: " + Session["donorNotes"]);
                    }
                    else
                    {
                        Models.Notes.InsertNotes(Session["donorNotes"].ToString(), type, note);
                        Models.Util.SaveAction("Insert Note: " + Session["donorNotes"]);
                    }
                    return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                }
            }
            catch (Exception ex)
            {
                ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
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
                    var notes = MPXMobile.Models.Notes.GetAccountNotes(Session["donorNotes"].ToString());
                    var id = Convert.ToInt64(noteId);
                    var noteCurrent = notes.accountNotes.Where(n => n.noteId == id).First();

                    if (noteCurrent != null)
                    {
                        var texts = noteCurrent.note.Split(':');
                        if (texts[0].ToLower().Contains(User.Identity.Name.ToLower()))
                        {
                            Models.Notes.DeleteNotes(int.Parse(noteId));
                            Models.Util.SaveAction("Delete Note: " + Session["donorNotes"]);
                            return RedirectToAction("NotesInformation", "Home", new { accountId = Session["donorNotes"].ToString() });
                        }
                        ViewData["Message"] = "You can only delete your own note.";
                    }
                    else
                    {
                        ViewData["Message"] = "That note is not exist.";
                    }

                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["ERRORPATH"]), ex.Message);
                    ViewData["Message"] = "That note is not exist.";
                }
                return View();
            }
        }
    }
}
