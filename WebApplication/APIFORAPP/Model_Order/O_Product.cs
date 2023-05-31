using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class O_Product : Result
    {
        public List<ItemProduct> Data { get; set; }
    }
    public class ItemProduct
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool isNew { get; set; }
        public string Thumnails { get; set; }
        public long? Cate { get; set; }
    }
}