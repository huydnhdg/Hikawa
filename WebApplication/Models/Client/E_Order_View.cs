using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Client
{
    public class E_Order_View : E_Order
    {
        public List<E_OderItems> Items { get; set; }
    }
}