using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class E_Product_Extra_Create : E_Product
    {
        public List<E_Product_Extra> Extras { get; set; }
    }
}