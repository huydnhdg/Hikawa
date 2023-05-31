using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class ArticleController : BaseController
    {
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Articles
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Title.StartsWith(textsearch));
                ViewBag.textsearch = textsearch;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            IEnumerable<Article> data = model as IEnumerable<Article>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            var cate = db.Article_Cate.OrderBy(a => a.Name);
            ViewBag.cate = cate;
            return View();
        }
        string domain = WebConfigurationManager.AppSettings["DOMAIN"];
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Article article)
        {
            try
            {
                article.Createdate = DateTime.Now;
                db.Articles.Add(article);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }
            var cate = db.Article_Cate.OrderBy(a => a.Name);
            ViewBag.cate = cate;
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        public ActionResult Edit(long Id)
        {
            var cate = db.Article_Cate.OrderBy(a => a.Name);
            ViewBag.cate = cate;
            var model = db.Articles.Find(Id);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Article article)
        {
            try
            {
                var _a = db.Articles.Find(article.Id);

                _a.Image = article.Image;
                _a.Link = article.Link;
                _a.Description = article.Description;
                _a.Detail = article.Detail;
                _a.Title = article.Title;
                _a.Tags = article.Tags;
                //nếu bài viết là banner thì không đổi loại bài viết
                if (_a.Cate != "banner")
                {
                    _a.Cate = article.Cate;
                }
                db.Entry(_a).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }
            var cate = db.Article_Cate.OrderBy(a => a.Name);
            ViewBag.cate = cate;
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        public ActionResult Detail(string url)
        {
            var model = db.Articles.Where(a => a.Link == url).SingleOrDefault();
            return View(model);
        }
    }
}