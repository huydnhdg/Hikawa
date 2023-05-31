using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Product_Export
{
    public class Export_Product : P_Export
    {
        public Export_Product()
        {
            this.Items = new List<P_Export_ItemView>();
        }
        public List<P_Export_ItemView> Items { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}