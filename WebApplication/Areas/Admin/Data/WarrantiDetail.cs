using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class WarrantiDetail : Warranti
    {
        public string CusName { get;set;}

        //sản phẩm
        public DateTime? Activedate { get; set; }
        public int? Limited { get; set; }
        public DateTime? Enddate { get; set; }

        public List<Warranti_Accessary> Warranti_Accessaries { get; set; }
        public List<Warranti_Image> Image { get; set; }
        //ten day du
        public string Station { get; set; }
        public string KTV { get; set; }
    }
}