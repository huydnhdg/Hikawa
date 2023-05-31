using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class O_Order:Result
    {
        public List<ListOrder> Data { get; set; }
    }
    public class ListOrder
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public DateTime? Createdate { get; set; }
        public int Amount { get; set; }
        public string LinkDowload { get; set; }
    }
}