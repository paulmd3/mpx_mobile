using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using MPXMobile.WipService;
using System.Text.RegularExpressions;
using System.Text;

namespace MPXMobile.Models
{
    //  The interface and helper class below 
    //  create an abstract wrapper around such a type 
    public interface IUseApiDonors
    {
        account GetAccount(long id);
        List<account> Search(SearchCondition condition);
        List<account> RecentlyDonors(account account);
        bool BookmarkDonor(BookmarkedDonor bookmark);
        List<BookmarkedDonor> BookmarkedByUser(string idUser);
        accountAddress[] GetAccountAddress(long id);
        accountEmailAddress[] GetAccountEmail(long id);
        accountPhoneNumber[] GetAccountPhone(long id);
    }

    /// <summary>
    /// Implement the interface methods
    /// </summary>
    public class ApiServiceDonors : IUseApiDonors
    {
        public bool UnBookmarkDonor(string idUser, long donorID)
        {
            try
            {
                UsersAutDataContext context = new UsersAutDataContext();
                List<BookmarkedDonor> list = (from a in context.BookmarkedDonors
                         where
                         (a.User_ID == idUser && a.Donor_ID == donorID)
                         select a).ToList<BookmarkedDonor>();
                if (list != null && list.Count > 0)
                {
                    context.BookmarkedDonors.DeleteOnSubmit(list[0]);
                    context.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public List<BookmarkedDonor> GetBookmarkedByUser(string idUser, int intDonorID)
        {
            UsersAutDataContext context = new UsersAutDataContext();
            var items = (from a in context.BookmarkedDonors
                         where
                         (a.User_ID == idUser && a.Donor_ID == intDonorID)
                         select a).ToList<BookmarkedDonor>();
            return items;
        }

        /// <summary>
        /// Verify and load the recently list of donors
        /// </summary>
        /// <param name="account">Save the donor comming at the end of the recently donors list</param>
        /// <returns>List of account</returns>
        public List<account> RecentlyDonors(account account)
        {
            List<account> listaccountcache = (List<account>)HttpRuntime.Cache.Get("listaccount");
            if (listaccountcache == null)
            {
                listaccountcache = new List<account>();
            }

            var validate = listaccountcache.Where(a => a.accountId.ToString() == account.accountId.ToString());
            if (validate.Count() == 0)
            {
                listaccountcache.Add(account);
            }

            HttpRuntime.Cache.Add("listaccount", listaccountcache, null, Cache.NoAbsoluteExpiration, new TimeSpan(7, 0, 0, 0), CacheItemPriority.Normal, null);
            return listaccountcache;
        }

        #region Bookmarked DB

        /// <summary>
        /// Save donors in Bookmarked table by user id using Linq datacontext to connect with MPX database
        /// </summary>
        /// <param name="bookmark">Bookmarked Donor object</param>
        /// <returns>If the transaction has been succesfull</returns>
        public bool BookmarkDonor(BookmarkedDonor bookmark)
        {
            var list = BookmarkedByUser(bookmark.User_ID);

            if (list.Where(r => r.Donor_ID == bookmark.Donor_ID).Count() == 0)
            {
                try
                {
                    UsersAutDataContext context = new UsersAutDataContext();
                    context.BookmarkedDonors.InsertOnSubmit(bookmark);
                    context.SubmitChanges();

                    Models.Util.SaveAction(Util.Actions.BookMarked + ":" + bookmark.Donor_ID + " status:true");
                    return true;
                }
                catch
                {
                    Models.Util.SaveAction(Util.Actions.BookMarked + ":" + bookmark.Donor_ID + " status:false");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Return the list of bookmarked donors by user
        /// using Linq datacontext and expression for the specific query
        /// </summary>
        /// <param name="idUser">Represents the user id</param>
        /// <returns>A list of bookmarked donors by user id</returns>
        public List<BookmarkedDonor> BookmarkedByUser(string idUser)
        {
            UsersAutDataContext context = new UsersAutDataContext();
            var items = (from a in context.BookmarkedDonors
                         where
                         (a.User_ID == idUser)
                         select a).ToList<BookmarkedDonor>();
            return items;
        }        

        #endregion


        #region AccountService


        /// <summary>
        /// Search will return all the list of donors and save it on cache
        /// it has been implemented to make the less possible calls to the Wip service 
        /// </summary>
        /// <returns> The cache result(getAccountsResponse) </returns>
        public List<account> Search(SearchCondition condition)
        {
            var port = Models.Util.Port();
            getAccountsRequest acreq = new getAccountsRequest();

            StringBuilder strAction = new StringBuilder();
            strAction.Append(Util.Actions.SearchFor.ToString());            

            if (condition.Email.Length > 0)
            {
                acreq.emailAddress = condition.Email;
                strAction.Append("Email:" + condition.Email);
            }
            if (condition.Zip.Length > 0)
            {
                acreq.zipCode = condition.Zip;
                strAction.Append("Zip:" + condition.Zip);
            }
            if (condition.Organization.Length > 0)
            {
                acreq.organization = condition.Organization;
                strAction.Append("Organization:" + condition.Organization);
            }
            if (condition.AccountNumber.Length > 0)
            {
                acreq.accountIdRangeStart = Convert.ToInt64(condition.AccountNumber);
                acreq.accountIdRangeEnd = Convert.ToInt64(condition.AccountNumber);
                strAction.Append("AccountNumber:" + condition.AccountNumber);
            }

            if (condition.Name.Length > 0)
            {                
                strAction.Append("Name:" + condition.Name);
            }

            Models.Util.SaveAction(strAction.ToString());

            if (condition.Name.Length == 0)
            {
                return port.getAccounts(acreq).accounts.ToList();
            }
            else if (condition.Name.Contains(" "))
            {
                var names = condition.Name.Split(' ');
                acreq.firstName = names[0];
                acreq.lastName = names[1];
                return port.getAccounts(acreq).accounts.ToList();
            }
            else
            {
                acreq.firstName = condition.Name;
                var first = port.getAccounts(acreq).accounts;

                acreq.firstName = null;
                acreq.lastName = condition.Name;
                var last = port.getAccounts(acreq).accounts.ToList();

                foreach (account a in first)
                {
                    var have = from acc in last where acc.accountId == a.accountId select acc;

                    if (have.Count() == 0)
                    {
                        last.Add(a);
                    }
                }

                return last;
            }
        }

        /// <summary>
        /// Returns a the description of some account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public account GetAccount(long id)
        {
            var port = Util.Port();
            WipService.getAccountRequest acreq = new getAccountRequest();
            acreq.accountId = id;
            return port.getAccount(acreq).account;
        }

        /// <summary>
        /// Returns all addresses of an account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public accountAddress[] GetAccountAddress(long id)
        {
            var port = Util.Port();
            getAccountAddressesRequest request = new getAccountAddressesRequest();
            request.accountId = id;
            return port.getAccountAddresses(request).accountAddresses;
        }

        /// <summary>
        /// Returns all emails of an account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public accountEmailAddress[] GetAccountEmail(long id)
        {
            var port = Util.Port();
            getAccountEmailAddressesRequest request = new getAccountEmailAddressesRequest();
            request.accountId = id;
            return port.getAccountEmailAddresses(request).accountEmailAddresses;
        }

        /// <summary>
        /// Returns all phones of an account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public accountPhoneNumber[] GetAccountPhone(long id)
        {
            var port = Util.Port();
            getAccountPhoneNumbersRequest request = new getAccountPhoneNumbersRequest();
            request.accountId = id;
            return port.getAccountPhoneNumbers(request).accountPhoneNumbers;
        }

        #endregion

        #region IUseApiDonors Members

        #endregion
    }
}
