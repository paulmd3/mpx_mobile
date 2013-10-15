using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace MPXMobile.Models
{
    /// <summary>
    /// Represents the local class, will to connect with the local 
    /// database using Linq datacontext
    /// </summary>
    public class Util
    {

        /// <summary>
        /// Validate the user credentials with the local db
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static LocalUser UserAut(string username, string password)
        {
            password = GetMD5(password);
            UsersAutDataContext context = new UsersAutDataContext();

            try
            {
                var items = (from a in context.LocalUsers
                             where
                               (a.Email == username && a.Password == password)
                             select a).Single();
                return items;
            }
            catch
            {
                return null;
            }

        }



        /// <summary>
        /// Verify if User exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static LocalUser VerifyUser(string username)
        {
            
            UsersAutDataContext context = new UsersAutDataContext();

            try
            {
                var items = (from a in context.LocalUsers
                             where
                               (a.Email == username)
                             select a).Single();
                return items;
            }
            catch
            {
                return null;
            }

        }



        /// <summary>
        /// Returns the user to update log tables
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static LocalUser LocalUser(string username, string password)
        {
            UsersAutDataContext context = new UsersAutDataContext();
            var items = (from a in context.LocalUsers
                         where
                         (a.Email == username && a.Password == password)
                         select a).Single();
            return items;

        }


        /// <summary>
        /// Update the history of the local user table
        /// </summary>
        /// <param name="userId"></param>
        public static void updateLogUser(string userId)
        {
            UsersAutDataContext context = new UsersAutDataContext();

            if (context.LocalUsers.Count() > 0)
            {
                LocalUser Base = context.LocalUsers.Single(a => a.Email == userId);
                Base.Updated_at = DateTime.Now;
                context.SubmitChanges();

            }
        }

        /// <summary>
        /// Change password
        /// </summary>
        public static bool ChangePassword(string userId, string password)
        {
            password = GetMD5(password);
            UsersAutDataContext context = new UsersAutDataContext();
            LocalUser Base = context.LocalUsers.Single(a => a.Email == userId);
            Base.Password = password;
            context.SubmitChanges();
            return true;

        }


        /// <summary>
        /// Save in the LogSessions table
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SaveLogIn(string userId)
        {
            UsersAutDataContext context = new UsersAutDataContext();
            LogSession ses = new LogSession();
            ses.SessionDate = DateTime.Now;
            ses.UserId = userId;
            context.LogSessions.InsertOnSubmit(ses);
            context.SubmitChanges();
        }

        /// <summary>
        /// Save in the logAction table
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SaveAction(string action)
        {
            UsersAutDataContext context = new UsersAutDataContext();
            LogAction actions = new LogAction();
            actions.Action = action;
            actions.UserId = HttpContext.Current.User.Identity.Name;
            actions.ActionDate = DateTime.Now;
            context.LogActions.InsertOnSubmit(actions);
            context.SubmitChanges();
        }

        /// <summary>
        /// Validate credentials to use the Wip service
        /// </summary>
        public static WipService.MpxWebServicesPort Port()
        {

            WipService.credentials credentials = new MPXMobile.WipService.credentials();
            credentials.password = System.Configuration.ConfigurationSettings.AppSettings["LogP"];
            credentials.userName = System.Configuration.ConfigurationSettings.AppSettings["LogU"];
            WipService.MpxWebServicesPort port = new MPXMobile.WipService.MpxWebServicesPort();
            port.userCredentials = credentials;
            return port;

        }

        /// <summary>
        /// Enumerator lists of actions
        /// </summary>
        public enum Actions
        {
            GetRecentlyDonors = 1,
            SearchFor = 2,
            GetDonorInformation = 3,
            BookMarked = 4,
            GetBookmarkedDonors = 5,
            GetDonorGiftInformation = 6

        }


        /// <summary>
        /// Create User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static bool CreateUser(LocalUser user)
        {
            try
            {
                UsersAutDataContext context = new UsersAutDataContext();
                context.LocalUsers.InsertOnSubmit(user);
                context.SubmitChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Encrypt password
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

    }
}
