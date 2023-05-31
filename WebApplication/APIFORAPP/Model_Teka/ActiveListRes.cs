using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class ActiveListRes : Result
    {
        public List<ActiveListModel> Data { get; set; }
    }
    public class ActiveListModel
    {
        public long Id { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string SerialBrand { get; set; }
        public DateTime? Activedate { get; set; }
        public DateTime? Buydate { get; set; }
        public int? Amount { get; set; }
        public DateTime? Thanhtoan { get; set; }
        public string AgentId { get; set; }
    }
}