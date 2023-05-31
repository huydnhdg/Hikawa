using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Order
{
    public class Order_Items : Result
    {
        public List<Items_New> Data { get; set; }
    }

    public class Items_New : E_OderItems
    {

    }
}