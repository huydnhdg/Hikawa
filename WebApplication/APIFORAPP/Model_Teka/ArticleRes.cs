using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class ArticleRes : Result
    {
        public List<TK_Article> Data { get; set; }
    }

    public class TK_Article
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public string Alt { get; set; }
        public string Text { get; set; }
        public string Cate { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string Tag { get; set; }
        public Nullable<int> Type { get; set; }
        public Nullable<int> Status { get; set; }
    }
}