using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class E_Order_Details : E_Order_Model
    {
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<E_OderItems> Items { get; set; }
    }
}