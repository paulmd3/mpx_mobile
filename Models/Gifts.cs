using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MPXMobile.WipService;

namespace MPXMobile.Models
{
    public interface IUseApiGifts
    {
        Dictionary<string, List<giftAnnual>> GetGift(string id);
        getGiftAnnualSummariesResponse GetGiftAnnualSummaries(long id);
        getGiftSummariesResponse GetGiftSummaries(long id);
    }

    public class ApiServiceGifts : IUseApiGifts
    {
        public getGiftAnnualSummariesResponse GetGiftAnnualSummaries(long id)
        {
            MpxWebServicesPort port = Util.Port();
            getGiftAnnualSummariesRequest request = new getGiftAnnualSummariesRequest();
            request.accountId = id;
            return port.getGiftAnnualSummaries(request);
        }

        public getGiftSummariesResponse GetGiftSummaries(long id)
        {
            MpxWebServicesPort port = Util.Port();
            getGiftSummariesRequest request = new getGiftSummariesRequest();
            request.accountId = id;
            return port.getGiftSummaries(request);
        }
        
        public Dictionary<string, List<giftAnnual>> GetGift(string id)
        {
            Util.SaveAction(Util.Actions.GetDonorGiftInformation.ToString() + ": " + id);

            long num = Convert.ToInt64(id);
            Dictionary<string, List<giftAnnual>> dictionary = new Dictionary<string, List<giftAnnual>>();
            List<giftAnnual> list = new List<giftAnnual>();
            decimal num2 = 0M;
            string str = string.Empty;
            var groupYear = GetGiftAnnualSummaries(num).giftAnnualSummaries.OrderByDescending(f => f.giftDateYear).GroupBy(g => g.giftDateYear);

            foreach (IGrouping<int, giftAnnual> grouping in groupYear)
            {
                str = string.Empty;
                list = new List<giftAnnual>();

                foreach (IGrouping<string, giftAnnual> grouping2 in grouping.GroupBy(y => y.currencyCode))
                {
                    num2 = 0M;
                    foreach (giftAnnual annual in grouping2)
                    {
                        num2 += annual.amount;
                        list.Add(annual);
                    }
                    str = string.Format("{0} {1:N2} {2} and", str, num2, grouping2.Key);
                    str = str.Remove(str.Length - 3);
                }

                str = string.Format("{0} in {1}", str, grouping.Key);
                dictionary.Add(str, list);
            }

            //foreach (IGrouping<int, giftAnnual> grouping in GetGiftAnnualSummaries(num).giftAnnualSummaries.GroupBy<giftAnnual, int>(delegate(giftAnnual a)
            //{
            //    return a.giftDateYear;
            //}))
            //{
            //    str = string.Empty;
            //    list = new List<giftAnnual>();
            //    foreach (IGrouping<string, giftAnnual> grouping2 in grouping.GroupBy<giftAnnual, string>(delegate(giftAnnual a)
            //    {
            //        return a.currencyCode;
            //    }))
            //    {
            //        num2 = 0M;
            //        foreach (giftAnnual annual in grouping2)
            //        {
            //            num2 += annual.amount;
            //            list.Add(annual);
            //        }
            //        str = string.Format("{0} {1:N2} {2} and", str, num2, grouping2.Key);
            //        str = str.Remove(str.Length - 3);
            //    }
            //    str = string.Format("{0} in {1}", str, grouping.Key);
            //    dictionary.Add(str, list);
            //}
            return dictionary;
        }
    }

    public class GiftInfo
    {
        public account Donor { get; set; }
        public Dictionary<string, List<giftAnnual>> Gifts { get; set; }
    }

    public class GiftYear
    {
        public giftAnnual Annual { get; set; }
        public List<giftSummary> Summary { get; set; }
    }
}
