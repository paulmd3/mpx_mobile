using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MPXMobile.WipService;

namespace MPXMobile.Models
{
    public class DonorDetail
    {
        public account Donor { get; set; }
        public List<accountAddress> Addresses { get; set; }
        public List<accountEmailAddress> Emails { get; set; }
        public List<accountPhoneNumber> Phones { get; set; }
    }
}