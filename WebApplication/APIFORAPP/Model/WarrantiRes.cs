using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Areas.Admin.Data;

namespace WebApplication.APIFORAPP.Model
{
    public class WarrantiRes : Result
    {
        public List<WarrantiModel> Data { get; set; }
    }
}