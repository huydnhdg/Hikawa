using Newtonsoft.Json;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class B_CateController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.B_Cate
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Name.ToString().Contains(textsearch)
                || a.Description.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            IEnumerable<B_Cate> data = model as IEnumerable<B_Cate>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public JsonResult GetAmount()
        {
            var voucher = from a in db.B_Voucher
                          group a by a.Amount into g
                          select new { Amount = g.FirstOrDefault().Amount, };
            return Json(voucher, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/B_Cate/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] B_Cate cate, int AmountTOPUP = 0)
        {
            try
            {
                if (cate.Code == "TOPUP")
                {
                    cate.Amount = AmountTOPUP;
                }
                var _cate = db.B_Cate.Where(a=> a.Code == cate.Code);
                if (_cate.Count() > 0)
                {
                    var _amount = _cate.Where(a => a.Amount == cate.Amount);
                    if (_amount.Count() > 0)
                    {
                        SetAlert("Lưu thông tin không thành công. Mệnh giá này đã được cấu hình.", "danger");
                        return RedirectToAction("Index");
                    }
                    var _point = _cate.Where(a => a.Point == cate.Point);
                    if (_point.Count() > 0)
                    {
                        SetAlert("Lưu thông tin không thành công. Điểm này đã được cấu hình.", "danger");
                        return RedirectToAction("Index");
                    }

                }
                cate.Createby = User.Identity.Name;
                cate.Createdate = DateTime.Now;
                db.B_Cate.Add(cate);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }

            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.B_Cate.Find(Id);
            return PartialView("~/Areas/Admin/Views/B_Cate/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] B_Cate cate, int AmountTOPUP = 0)
        {
            try
            {
                if (cate.Code == "TOPUP")
                {
                    cate.Amount = AmountTOPUP;
                }
                db.Entry(cate).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.B_Cate.Find(Id);

                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @Cate={1}", User.Identity.Name, json));

                db.B_Cate.Remove(model);
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