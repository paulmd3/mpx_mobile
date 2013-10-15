using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using System.Threading;
using System.Net.Mail;
using System.IO;
using System.Text;

namespace MPXMobile.Controllers
{

    [HandleError]
    public class AccountController : BaseController
    {

        // This constructor is used by the MVC framework to instantiate the controller using
        // the default forms authentication and membership providers.

        public AccountController()
            : this(null, null)
        {
        }

        // This constructor is not used by the MVC framework but is instead provided for ease
        // of unit testing this type. See the comments at the end of this file for more
        // information.
        public AccountController(IFormsAuthentication formsAuth, IMembershipService service)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationService();
            MembershipService = service ?? new AccountMembershipService();
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public IMembershipService MembershipService
        {
            get;
            private set;
        }

        public ActionResult LogOn()
        {
            string strName = string.Empty;
            if (Request.Cookies.AllKeys.Contains("User"))
            {
                strName= Request.Cookies.Get("User").Value;
            }

            ViewData["User"] = strName;
            
            if (Request.IsAjaxRequest())
            {
                return View("_LogOn");
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, bool rememberMe, string returnUrl)
        {
            string strName = string.Empty;
            if (Request.Cookies.AllKeys.Contains("User"))
            {
                strName = Request.Cookies.Get("User").Value;
            }

            ViewData["User"] = strName;

            if (!ValidateLogOn(userName.Trim(), password.Trim()))
            {
                return View();
            }

            //var validate = Models.Users.UserAut(userName, password);

            FormsAuth.SignIn(userName, rememberMe);

            if (!String.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                ///save log history
                ///update localuser table
                //var Id = Models.Util.UserAut(userName, password).Email;

                Models.Util.SaveLogIn(userName.Trim());
                Models.Util.updateLogUser(userName.Trim());
                Session.Add("user", userName.Trim());
                Session.Add("pass", password.Trim());

                if (rememberMe)
                {
                    HttpCookie cookie = new HttpCookie("User");
                    cookie.Expires = DateTime.Now.AddYears(1);
                    cookie.Value = userName.Trim();
                    Response.Cookies.Add(cookie);
                }
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult LogOff()
        {
            FormsAuth.SignOut();

            return RedirectToAction("LogOn", "Account");
        }

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Verify if user exists.
        /// Read .txt file, it represents the emails body
        /// Create the new password
        /// Send email
        /// </summary>
        /// <param name="username">Email</param>
        /// <returns>Updates the old password and send a mail with the new password</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotPassword(string username)
        {
            var verify = Models.Util.VerifyUser(username);

            if (verify != null)
            {
                string newPassword = CreateRandomPassword(6);
                string html = string.Empty;
                String line;
                try
                {
                    //Pass the file path and file name to the StreamReader constructor

                    String strFilePath = (Server.MapPath("/Content/mail.txt"));
                    StreamReader sr = new StreamReader(strFilePath);

                    //Read the first line of text
                    line = sr.ReadLine();

                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        //Read the next line
                        line = sr.ReadLine();
                        html = html + line;
                    }

                    //close the file
                    sr.Close();

                    //Change the new password line
                    StringBuilder sb = new StringBuilder(html);
                    sb.Replace("NEWPASSWORD", newPassword.ToString());
                    html = sb.ToString();

                }
                catch (Exception ex)
                {
                    ErrorLog(Server.MapPath(System.Configuration.ConfigurationSettings.AppSettings["ERRORPATH"]), ex.Message);
                    return View("Error");
                }

                var message = new MailMessage(username, username)
                {
                    Subject = "MPX New password",
                    Body = html
                };
                message.IsBodyHtml = true;
                var client = new SmtpClient();
                client.EnableSsl = true;
                Models.Util.ChangePassword(verify.Email, newPassword);
                client.Send(message);

            }
            return RedirectToAction("LogOn", "Account");
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string firstname, string lastname, string email, string password, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (ValidateRegistration(email, password, confirmPassword))
            {
                // Attempt to register the user
                var createStatus = MembershipService.CreateUser(firstname, lastname, password, email);


                if (createStatus)
                {
                    FormsAuth.SignIn(email, false /* createPersistentCookie */);
                    return RedirectToAction("Search", "Home");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "A username for that e-mail address already exists. Please enter a different e-mail address.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Exceptions result in password not being changed.")]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(User.Identity.Name, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
        }


        public ActionResult DefaultMenu()
        {

            return RedirectToAction("Defaultmenu", "Home");

        }
        public ActionResult ChangePasswordSuccess()
        {

            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods

        private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            return ModelState.IsValid;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Change return true
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }
            if (!MembershipService.ValidateUser(userName, password))
            {
                ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
            }

            return ModelState.IsValid;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private bool ValidateRegistration(string email, string password, string confirmPassword)
        {

            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < MembershipService.MinPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         MembershipService.MinPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }
            return ModelState.IsValid;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        //MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool CreateUser(string firstname, string lastname, string password, string email);
    }

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (Models.Util.UserAut(userName, password) != null)
            {

                return true;
            }
            else
                return false;
            //return _provider.ValidateUser(userName, password);
        }

        public bool CreateUser(string firstname, string lastname, string password, string email)
        {
            //MembershipCreateStatus status;
            password = Models.Util.GetMD5(password);
            Models.LocalUser newUser = new MPXMobile.Models.LocalUser();
            newUser.Email = email;
            newUser.Password = password;
            newUser.First_Name = firstname;
            newUser.Last_name = lastname;
            newUser.Updated_at = DateTime.Now;
            newUser.Created_at = DateTime.Now;
            var ret = Models.Util.CreateUser(newUser);
            //_provider.CreateUser(password, email, null, null, true, null, out status);
            return ret;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var currentUser = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            return Models.Util.ChangePassword(currentUser, newPassword);
        }
    }
}
