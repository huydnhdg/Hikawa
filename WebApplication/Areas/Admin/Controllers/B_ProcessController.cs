using Newtonsoft.Json;
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
    public class B_ProcessController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.B_Process
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Startdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Startdate <= s);
                ViewBag.end_date = end_date;
            }
            IEnumerable<B_Process> data = model as IEnumerable<B_Process>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Startdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/B_Process/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] B_Process process)
        {
            if (ModelState.IsValid)
            {
                var _pro = db.B_Process.Where(a => a.CODE == process.CODE);
                if (_pro.Count() > 0)
                {
                    SetAlert("Mã chương trình đã tồn tại. Lưu thông tin không thành công.", "danger");
                    return RedirectToAction("Index");
                }
                process.Createdate = DateTime.Now;
                process.Createby = User.Identity.Name;
                db.B_Process.Add(process);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.B_Process.Find(Id);
            return PartialView("~/Areas/Admin/Views/B_Process/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] B_Process process)
        {
            if (ModelState.IsValid)
            {
                db.Entry(process).State = EntityState.Modified;
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
                var model = db.B_Process.Find(Id);

                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @Process={1}", User.Identity.Name, json));

                db.B_Process.Remove(model);
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