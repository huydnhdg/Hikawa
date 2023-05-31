using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Product_Export
{
    public class Import_Product: P_Import
    {
        public Import_Product()
        {
            this.Items = new List<P_Import_Item>();
        }
        public List<P_Import_Item> Items { get; set; }
    }
}