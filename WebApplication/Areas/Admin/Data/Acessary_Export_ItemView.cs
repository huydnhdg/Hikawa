using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class Acessary_Export_ItemView: Acessary_Export_Item
    {
        public string Code_Export { get; set; }
        public string UserKey { get; set; }
    }
}