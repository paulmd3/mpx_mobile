using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MPXMobile.Models
{
    public class SearchCondition
    {
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
    }
}