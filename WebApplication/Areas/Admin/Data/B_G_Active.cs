using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class B_G_Active
    {
        public string Model { get; set; }
        public string ProductCode { get; set; }
        public DateTime? Activedate { get; set; }
        public string Phone { get; set; }
        public List<String> Processs { get; set; }
        public List<String> Point { get; set; }
        public bool Status { get; set; }
        public DateTime? CheckActive { get; set; }

        public string PayContent { get; set; }
        public int? PayAmount { get; set; }
        public string PayCate { get; set; }

        public bool? isActive { get; set; }//true = active
    }
}