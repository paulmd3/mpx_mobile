using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.IO;
using System.Net.Mail;

namespace MPXMobile.Controllers
{
    public class BaseController : Controller
    {
        private string sLogFormat;
        private string sErrorTime;

        public bool IsAjaxRequest()
        {
            if (!String.IsNullOrEmpty(Request.Headers["Ajax"]) ||
                "XMLHttpRequest".Equals(Request.Headers["X-Requested-With"],
                StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            if (!String.IsNullOrEmpty(Request.Headers["X-IsJson"]))
            {
                return true;
            }
            else
                return false;
        }

        public void ErrorLog(string sPathName, string sErrMsg)
        {
            if (!System.IO.Directory.Exists(sPathName))
            {

                var fs = new FileStream(sPathName, FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(sLogFormat + sErrMsg);
                sw.Flush();
                sw.Close();
            }
        }
        /// <summary>
        /// Generate a Random Password
        /// </summary>
        /// <param name="passwordLength"></param>
        /// <returns></returns>
        public string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }


        /// <summary>
        /// Method to send an email
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="toAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public string SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            try
            {
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                message.IsBodyHtml = true;
                var client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(message);
                return "k";
            }
            catch (Exception e)
            {
                return e.Message;
                throw;
            }
        }

    }
}
