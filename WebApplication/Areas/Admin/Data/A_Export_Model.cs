using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class A_Export_Model : Accessary_Export
    {
        public string Address { get; set; }
        public string KeyName { get; set; }
        public List<Acessary_Export_Item> ListItem { get; set; }
    }
}