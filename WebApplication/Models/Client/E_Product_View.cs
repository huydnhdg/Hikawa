using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Client
{
    public class E_Product_View:E_Product
    {
        public string CateName { get; set; }
        public string CateLink { get; set; }
        public List<E_Product_Extra> Extra { get; set; }
        public List<E_Product> ListProduct { get; set; }
        public List<E_Product_Image> ListImage { get; set; }
    }
}