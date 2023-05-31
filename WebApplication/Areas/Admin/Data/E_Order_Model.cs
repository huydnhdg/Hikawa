using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class E_Order_Model : E_Order
    {
        public string AgentName { get; set; }
        public int CountProduct { get; set; }
        public int SumAmount { get; set; }

    }
}