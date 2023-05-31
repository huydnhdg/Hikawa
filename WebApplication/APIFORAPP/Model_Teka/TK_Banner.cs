using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class TK_Banner : Result
    {
        public List<Banner> Data { get; set; }
    }
    public class Banner
    {
        public long Id { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }
    }
}