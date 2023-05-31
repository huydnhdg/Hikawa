using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class Article_Cate : Result
    {
        public List<Article_Cate_Model> Data { get; set; }
    }
    public class Article_Cate_Model
    {
        public string Cate { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}