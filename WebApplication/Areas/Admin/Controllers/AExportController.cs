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
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class AExportController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //ELMEntities db = new ELMEntities();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Accessary_Export
                        select new A_Export_Model()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Id_Key = a.Id_Key,
                            Createdate = a.Createdate,
                            Note = a.Note,
                            ListItem = db.Acessary_Export_Item.Where(l => l.Id_Export == a.Id).ToList()
                        };
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Id_Key.Contains(textsearch)
                || a.Note.Contains(textsearch));
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
            IEnumerable<Accessary_Export> data = model as IEnumerable<Accessary_Export>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.Accessary_Export.Find(Id);
                var listitem = db.Acessary_Export_Item.Where(a => a.Id_Export == model.Id);
                if (listitem.Count() > 0)
                {
                    SetAlert("Phiếu xuất có linh kiện, không xóa được.", "warning");
                }
                else
                {
                    string json = JsonConvert.SerializeObject(model);
                    logger.Info(string.Format("[Delete] @UserName={0} @Product={1}", User.Identity.Name, json));
                    db.Accessary_Export.Remove(model);
                    db.SaveChanges();
                    SetAlert("Đã xóa thành công.", "success");
                }
                
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult List(long Id)
        {
            var export = from a in db.Accessary_Export
                         join b in db.AspNetUsers on a.Id_Key equals b.UserName
                         select new A_Export_Model()
                         {
                             Id = a.Id,
                             Code = a.Code,
                             Id_Key = a.Id_Key,
                             KeyName = b.FullName,
                             Address = b.Address + " " + b.Ward + " " + b.District + " " + b.Province,
                             Createdate = a.Createdate,
                             Note = a.Note,
                             ListItem = db.Acessary_Export_Item.Where(l => l.Id_Export == a.Id).ToList()
                         };
            var model = export.FirstOrDefault(z => z.Id == Id);
            return PartialView("~/Areas/Admin/Views/AExport/_List.cshtml", model);
        }
        public ActionResult Edit(long Id)
        {
            var export = from a in db.Accessary_Export
                         join b in db.AspNetUsers on a.Id_Key equals b.UserName
                         select new A_Export_Model()
                         {
                             Id = a.Id,
                             Code = a.Code,
                             Id_Key = a.Id_Key,
                             KeyName = b.FullName,
                             Address = b.Address + " " + b.Ward + " " + b.District + " " + b.Province,
                             Createdate = a.Createdate,
                             Note = a.Note,
                             ListItem = db.Acessary_Export_Item.Where(l => l.Id_Export == a.Id).ToList()
                         };
            var model = export.FirstOrDefault(z => z.Id == Id);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] A_Export_Model model)
        {
            try
            {
                var export = db.Accessary_Export.Find(model.Id);
                var listitem = db.Acessary_Export_Item.Where(a => a.Id_Export == model.Id);
                export.Id_Key = model.Id_Key;
                export.Note = model.Note;
                export.Code = model.Code;
                db.Entry(export).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
                SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
                return RedirectToAction("Index");
            }
        }
    }
}