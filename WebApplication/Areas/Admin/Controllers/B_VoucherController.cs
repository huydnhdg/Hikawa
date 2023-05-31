using Newtonsoft.Json;
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
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class B_VoucherController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.B_Voucher
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Amount.ToString().Contains(textsearch)
                || a.PhoneActive.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(status))
            {
                model = model.Where(a => a.Status.ToString() == status);
                ViewBag.status = status;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Activedate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Activedate <= s);
                ViewBag.end_date = end_date;
            }
            IEnumerable<B_Voucher> data = model as IEnumerable<B_Voucher>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/B_Voucher/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] B_Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.Code = voucher.Code.ToUpper();
                voucher.Createby = User.Identity.Name;
                voucher.Createdate = DateTime.Now;
                db.B_Voucher.Add(voucher);
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
            var model = db.B_Voucher.Find(Id);
            return PartialView("~/Areas/Admin/Views/B_Voucher/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] B_Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                voucher.Code = voucher.Code.ToUpper();
                db.Entry(voucher).State = EntityState.Modified;
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
                var model = db.B_Voucher.Find(Id);

                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @Voucher={1}", User.Identity.Name, json));

                db.B_Voucher.Remove(model);
                db.SaveChanges();
                SetAlert("Đã xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }
        public ActionResult UploadFile()
        {
            List<B_Voucher> list_product = new List<B_Voucher>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<B_Voucher> list_product = new List<B_Voucher>();
            try
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            string code;
                            string amount;
                            string cate;

                            try { code = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { code = ""; }
                            try { amount = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { amount = "0"; }
                            try { cate = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { cate = ""; }
                            

                            //add thong tin rows vao product
                            var voucher = new B_Voucher()
                            {
                                Code = code.ToUpper(),
                                Amount = int.Parse(amount),
                                Cate = cate,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name
                            };
                            //check trung serial code
                            if (!string.IsNullOrEmpty(code))
                            {
                                var _voucher = db.B_Voucher.Where(a => a.Code == code);
                                if (_voucher.Count() == 0 && int.Parse(amount) > 0)
                                {
                                    db.B_Voucher.Add(voucher);
                                    db.SaveChanges();
                                }

                            }
                            list_product.Add(voucher);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
            return View(list_product);
        }
    }
}