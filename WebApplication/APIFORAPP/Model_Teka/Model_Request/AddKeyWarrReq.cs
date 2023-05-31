using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class AddKeyWarrReq
    {
        public long IDWarranti { get; set; }
        public string KeyWarranti { get; set; }
        public int Status { get; set; }
        public DateTime? Deadline { get; set; }
    }
}