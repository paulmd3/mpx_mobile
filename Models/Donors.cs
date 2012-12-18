using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace MPXMobile.Models
{
    /// <summary>
    /// Donors Class
    /// </summary>
    public class Donors
    {


        private IUseApiDonors ApiServicesDonors
        {
            get;
            set;
        }


        #region Bookmarked DB


        /// <summary>
        /// Connects to the interface and attempts to apply the query.
        /// See the comments of the implemented method named BookmarkedByUser in ApiServiceDonors for more
        /// information.
        /// Save the action in db using Util.cs
        /// </summary>
        /// <param name="idUser">User Id passed by reference to provide the param to know who is the user to search by id in db</param>
        /// <returns>A list of Bookmarked donors by user</returns>
        public static List<BookmarkedDonor> BookmarkedByUser(string idUser)
        {

            ApiServiceDonors x = new ApiServiceDonors();
            Models.Util.SaveAction(Util.Actions.GetBookmarkedDonors.ToString());
            return x.BookmarkedByUser(idUser);
        }



        /// <summary>
        /// Save donors in db by userId
        /// Save the user actions
        /// See the comments of the implemented method named BookmarkDonor in ApiServiceDonors for more
        /// information.
        /// </summary>
        /// <param name="bookmark">Recive an BookmarkedDonor object</param>
        /// <returns>If the transaction has been succeced or not</returns>
        public static bool BookmarkDonor(BookmarkedDonor bookmark)
        {
            
            ApiServiceDonors x = new ApiServiceDonors();
            var list=x.BookmarkedByUser(bookmark.User_ID.ToString());
           var verify=list.Where(r=> r.Donor_ID ==bookmark.Donor_ID);
           if (verify.Count() == 0)
           {
               var donor = x.BookmarkDonor(bookmark);
               Models.Util.SaveAction(Util.Actions.BookMarked + ":" + bookmark.Donor_ID + " status:" + donor.ToString() + "");
               return donor;
           }
           return false;
        }


        #endregion


        /// <summary>
        /// Returns a the description of some account
        /// Review if the list of donors exists in a cache var and make the queries over this object else
        /// call the interface method 
        /// See the comments of the implemented method named Search in ApiServiceDonors for more
        /// information.
        /// </summary>
        /// <param name="id">Account id to make the select query where account_id=id and recive a single object</param>
        /// <returns>Donor account object</returns>
        public static WipService.account GetAccount(string id)
        {
            MPXMobile.WipService.getAccountsResponse cache = (MPXMobile.WipService.getAccountsResponse)HttpRuntime.Cache.Get("donors");
            if (cache == null)
            {
                ApiServiceDonors x = new ApiServiceDonors();
                x.Search();
            }
            var donor = ((MPXMobile.WipService.getAccountsResponse)HttpRuntime.Cache.Get("donors")).accounts.Where(x => x.accountId.ToString() == id).Single();
            return donor;

        }


        /// <summary>
        /// Load and returns list of recently Donors using the interface method
        /// See the comments of the implemented method named RecentlyDonors in ApiServiceDonors for more
        /// information.
        /// </summary>
        /// <param name="id">Recive the account information of the most recent donor and save it in the list of recent donors</param>
        /// <returns>List of recently donors</returns>
        public static List<MPXMobile.WipService.account> Recently_Donors(WipService.account account)
        {
            ApiServiceDonors x = new ApiServiceDonors();
            return x.RecentlyDonors(account);
        }


        /// <summary>
        /// Review if the list of donors exists in cache
        /// if exists the query will be done over the list
        /// else loads the cache list using the interface method
        /// See the comments of the implemented method named Search in ApiServiceDonors for more
        /// information.
        /// </summary>
        /// <param name="keyword">Search keyword parameter</param>
        /// <returns>Returns a list of accounts filtred by keyword</returns>
        public static List<MPXMobile.WipService.account> SearchDonors(string keyword)
        {
            List<MPXMobile.WipService.account> accountsFilter;
            MPXMobile.WipService.getAccountsResponse cache = (MPXMobile.WipService.getAccountsResponse)HttpRuntime.Cache.Get("donors");
            if (cache == null)
            {
                ApiServiceDonors x = new ApiServiceDonors();
                x.Search();
            }

            accountsFilter = ((MPXMobile.WipService.getAccountsResponse)HttpRuntime.Cache.Get("donors")).accounts.Where(x => x.lastName.ToLower().Contains(keyword.ToLower()) || x.defaultEmailAddress.ToLower().Contains(keyword.ToLower())
                                           || x.firstName.ToLower().Contains(keyword.ToLower())).ToList<MPXMobile.WipService.account>();
            Models.Util.SaveAction(Util.Actions.SearchFor.ToString() + keyword);
            return accountsFilter;

        }


        //  The interface and helper class below 
        //  create an abstract wrapper around such a type 

        public interface IUseApiDonors
        {

            WipService.getAccountResponse GetAccount(string id);
            MPXMobile.WipService.getAccountsResponse Search();
            List<MPXMobile.WipService.account> RecentlyDonors(MPXMobile.WipService.account account);
            bool BookmarkDonor(BookmarkedDonor bookmark);
            List<BookmarkedDonor> BookmarkedByUser(string idUser);

        }

        /// <summary>
        /// Implement the interface methods
        /// </summary>
        public class ApiServiceDonors : IUseApiDonors
        {

            /// <summary>
            /// Verify and load the recently list of donors
            /// </summary>
            /// <param name="account">Save the donor comming at the end of the recently donors list</param>
            /// <returns>List of MPXMobile.WipService.account</returns>
            public List<MPXMobile.WipService.account> RecentlyDonors(MPXMobile.WipService.account account)
            {
                List<MPXMobile.WipService.account> listaccountcache = (List<MPXMobile.WipService.account>)HttpRuntime.Cache.Get("listaccount");
                if (listaccountcache == null)
                {
                    listaccountcache = new List<MPXMobile.WipService.account>();
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
                try
                {
                    UsersAutDataContext context = new UsersAutDataContext();
                    context.BookmarkedDonors.InsertOnSubmit(bookmark);
                    context.SubmitChanges();
                    return true;
                }
                catch
                {
                    return false;
                }

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
            /// <returns> The cache result(MPXMobile.WipService.getAccountsResponse) </returns>
            public MPXMobile.WipService.getAccountsResponse Search()
            {
                var port = Models.Util.Port();
                WipService.getAccountsRequest acreq = new MPXMobile.WipService.getAccountsRequest();
                var accounts = port.getAccounts(acreq);
                HttpRuntime.Cache.Insert("donors", accounts, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(7));
                return (MPXMobile.WipService.getAccountsResponse)HttpRuntime.Cache.Get("donors");
            }


            /// <summary>
            /// Returns a the description of some account
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public WipService.getAccountResponse GetAccount(string id)
            {
                var port = Util.Port();
                WipService.getAccountRequest acreq = new MPXMobile.WipService.getAccountRequest();
                acreq.accountId = int.Parse(id);
                var account = port.getAccount(acreq);
                return account;
            }


            #endregion




        }






    }
}
