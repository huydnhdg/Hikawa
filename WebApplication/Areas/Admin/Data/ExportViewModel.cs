using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Areas.Admin.Data
{
    public class ExportViewModel
    {
        public string Code { get; set; }
        public string Station { get; set; }
        public string Item_Code { get; set; }
        public string Quantity { get; set; }
        public string Cate { get; set; }
        public string Orderdate { get; set; }
        public string Note { get; set; }
        public string Error { get; set; }
    }
}