using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Areas.Admin.Data
{
    public class PrintModel
    {
        public string Code { get; set; }
        public string Station { get; set; }
        public string StationPhone { get; set; }
        public string StationAddress { get; set; }
        public string CusName { get; set; }
        public string CusPhone { get; set; }
        public string CusAddress { get; set; }
        public string ProdName { get; set; }
        public string ProdCode { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public string Buydate { get; set; }
        public string BuyAdr { get; set; }
        public string CusPhoneExtra { get; set; }
        public string Note { get; set; }
        public string Cate { get; set; }
        public string Warranti_Cate { get; set; }
        public string PhuTung_Recevice { get; set; }
        public string Createdate { get; set; }
        public string Extra { get; set; }
        public string Successnote { get; set; }
        public string Amount { get; set; }
        public List<PhuTungPrint> PhuTung_Export { get; set; }
    }
    public class PhuTungPrint
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}