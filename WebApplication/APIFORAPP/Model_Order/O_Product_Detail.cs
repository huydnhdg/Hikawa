using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class O_Product_Detail : Result
    {
        public List<Items> Data { get; set; }
    }
    public class Items : E_Product
    {
        public List<String> Images { get; set; }
        public List<E_Product_Extra> Extras { get; set; }
    }
}