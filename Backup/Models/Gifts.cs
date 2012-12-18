using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPXMobile.Models
{
    public class Gifts
    {

        private IUseApiGifts ApiServicesGifts
        {
            get;
            set;
        }


        public static WipService.getGiftSummariesResponse GetGiftSummaries(string id)
        {
            ApiServiceGifts gifts = new ApiServiceGifts();
            Models.Util.SaveAction(Util.Actions.GetDonorGiftInformation.ToString() + ": " + id);
            return gifts.GetGiftSummaries(id);
        }


        //  The interface and helper class below 
        //  create an abstract wrapper around such a type 

        public interface IUseApiGifts
        {
            WipService.getGiftSummariesResponse GetGiftSummaries(string id);
        }


        public class ApiServiceGifts : IUseApiGifts
        {
            /// <summary>
            /// Returns a Gift Summaries by account
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public WipService.getGiftSummariesResponse GetGiftSummaries(string id)
            {
                var port = Models.Util.Port();
                WipService.getGiftSummariesRequest acreq = new MPXMobile.WipService.getGiftSummariesRequest();
                acreq.accountId = long.Parse(id.ToString());
                var account = port.getGiftSummaries(acreq);
                return account;
            }
        }

    }
}
