using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class OrderProductCateController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.E_ProductCate
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.StartsWith(textsearch)
                || a.Description.Contains(textsearch));
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
            IEnumerable<E_ProductCate> data = model as IEnumerable<E_ProductCate>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Sort).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/OrderProductCate/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] E_ProductCate model_cate)
        {
            try
            {
                model_cate.Createdate = DateTime.Now;
                db.E_ProductCate.Add(model_cate);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
                return RedirectToAction("Index");
            }
            

        }
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var e_ProductCate = db.E_ProductCate.Find(Id);
            return PartialView("~/Areas/Admin/Views/OrderProductCate/_Edit.cshtml", e_ProductCate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] E_ProductCate e_ProductCate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(e_ProductCate).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.E_ProductCate.Find(Id);
                db.E_ProductCate.Remove(model);
                db.SaveChanges();
                SetAlert("Đã xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }
    }
}