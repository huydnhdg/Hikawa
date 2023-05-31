using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;
using WebApplication.Models.Client;

namespace WebApplication.APIFORAPP.Model
{
    public class ProductActiveRes : Result
    {
        public List<ProductResult> Data { get; set; }
        public List<ListActive> ListActives { get; set; }
    }
}