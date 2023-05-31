using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class B_Model_List : B_Model
    {
        public List<MapProcessModel> Data { get; set; }
    }
    public class MapProcessModel{
        public long Id { get; set; }
        public string IdProcess { get; set; }
        public bool Status { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}