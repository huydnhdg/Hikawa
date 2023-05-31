using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Areas.Admin.Data;

namespace WebApplication.APIFORAPP.Model
{
    public class CustomerRes : Result
    {
        public List<CustomerResult> Data { get; set; }
    }
}