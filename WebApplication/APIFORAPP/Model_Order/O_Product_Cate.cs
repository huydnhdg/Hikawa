using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class O_Product_Cate : Result
    {
        public List<ItemsProductCate> Data { get; set; }
    }
    public class ItemsProductCate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Descripttion { get; set; }
        public int Sort { get; set; }
        public string Thumnails { get; set; }
    }

}