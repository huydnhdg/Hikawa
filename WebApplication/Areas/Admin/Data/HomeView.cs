using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Areas.Admin.Data
{
    public class HomeView
    {
        public int baohanh { get; set; }
        public int kichhoat { get; set; }
        public int taikhoan { get; set; }
        public int sanpham { get; set; }

        public int ac_SMS { get; set; }
        public int ac_APP { get; set; }
        public int ac_WEB { get; set; }

        public float ac_SMS_per { get; set; }
        public float ac_APP_per { get; set; }
        public float ac_WEB_per { get; set; }

        public int wa_SMS { get; set; }
        public int wa_APP { get; set; }
        public int wa_WEB { get; set; }
        public int wa_CMS { get; set; }

        public float wa_SMS_per { get; set; }
        public float wa_APP_per { get; set; }
        public float wa_WEB_per { get; set; }
        public float wa_CMS_per { get; set; }

        public List<KeyReport> keyreport { get; set; }
    }
    public class KeyReport
    {
        public string Name { get; set; }
        public int Warranti_Create { get; set; }
        public int Warranti_Receive { get; set; }
        public int? Amount { get; set; }
    }
}