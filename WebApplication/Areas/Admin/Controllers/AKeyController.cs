using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class AKeyController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //ELMEntities db = new ELMEntities();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Accessary_Key
                        select new A_K_Model()
                        {
                            Id = a.Id,
                            Id_Key = a.Id_Key,
                            Code = a.Code,
                            CountExport = a.CountExport,
                            CountImport = a.CountImport,
                            Exist = a.CountImport - a.CountExport,
                            KeyPrice = a.KeyPrice,
                            Name = a.Name,
                            ProductName = a.ProductName,
                            Discount = a.Discount
                        };
            if (!User.IsInRole("Admin - Quản trị toàn hệ thống"))
            {
                var cr_user = db.AspNetUsers.SingleOrDefault(a=>a.UserName == User.Identity.Name);
                model = model.Where(a => a.Id_Key == User.Identity.Name || a.Id_Key==cr_user.Createby);
            }

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Id_Key.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.ProductName.Contains(textsearch)
                || a.KeyPrice.ToString().Contains(textsearch)
                || a.Discount.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            IEnumerable<A_K_Model> data = model as IEnumerable<A_K_Model>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Accessary_Key.Find(Id);
            return PartialView("~/Areas/Admin/Views/AKey/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Accessary_Key accessary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accessary).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Liên hệ ngày quản trị hệ thống", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult ListImport(string Id)
        {
            var import = db.Acessary_Log_Key.Where(i => i.Id_Akey == Id && i.Type == 1);
            return PartialView("~/Areas/Admin/Views/AKey/_ListImport.cshtml", import.ToList());
        }

        [HttpPost]
        public ActionResult ListExport(string Id)
        {
            var import = db.Acessary_Log_Key.Where(i => i.Id_Akey == Id && i.Type == 2);
            return PartialView("~/Areas/Admin/Views/AKey/_ListExport.cshtml", import.ToList());
        }

    }
}