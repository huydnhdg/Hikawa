using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class TK_Prodres : Result
    {
        public List<TK_Product> Data { get; set; }
    }
    public class TK_Product
    {
        public long Id { get; set; }
        public string BuyAdr { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Serial { get; set; }
        public string Model { get; set; }
        public DateTime? Activedate { get; set; }
        public Nullable<int> Limited { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string PhoneActive { get; set; }
        public string PhoneSend { get; set; }
    }
}