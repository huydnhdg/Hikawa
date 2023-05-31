using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication.Areas.Admin.Controllers
{
    public class ConvertDBController : BaseController
    {
        // GET: Admin/ConvertDB
        public ActionResult Index()
        {
            var product = db.Products.ToList();
            foreach (var item in product)
            {
                string trim = item.Code.Trim();
                var _prod = db.Products.Find(item.Id);
                _prod.Code = trim;
                _prod.Serial = trim;
                db.Entry(_prod).State = EntityState.Modified;
            }
            db.SaveChanges();
            return View();
        }
    }
}