using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class O_Order_Req
    {
        public string UserName { get; set; }
        public List<ProductItems> Items { get; set; }
    }
    public class ProductItems
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public int Quantity { get; set; }
    }
}