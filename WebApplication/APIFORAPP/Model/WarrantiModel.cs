using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model
{
    public class WarrantiModel
    {
        public string Code { get; set; }
        public string ProductCode { get; set; }
        public string Status { get; set; }
        public string Deadline { get; set; }
        public string Station_Warranti { get; set; }
        public string KTV_Warranti { get; set; }
        public string Successdate { get; set; }
        public string Successnote { get; set; }
        public string Createdate { get; set; }
    }
}