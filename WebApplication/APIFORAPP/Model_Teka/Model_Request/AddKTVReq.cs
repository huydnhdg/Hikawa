using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class AddKTVReq
    {
        public string IDKTV { get; set; }
        public Nullable<long> IDWarranti { get; set; }
    }
}