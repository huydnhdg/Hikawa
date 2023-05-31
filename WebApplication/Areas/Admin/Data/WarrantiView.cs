using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class WarrantiView : Warranti
    {
        public string CusName { get; set; }

        
        public string CodeProduct { get; set; }
        public string ProductName { get; set; }
        public int? Limited { get; set; }
        public string ProductCate { get; set; }
        public string Trademark { get; set; }
        public string Active_date { get; set; }
        public string End_date { get; set; }
        public string Active_chanel { get; set; }

    }
}