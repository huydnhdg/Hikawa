using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
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
    public class ProductStampsController : BaseController
    {
        static List<ProductStamp> listerror = new List<ProductStamp>();
        // GET: Admin/ProductStamps
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.ProductStamps
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Serial.Contains(textsearch));
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/ProductStamps/_Create.cshtml", null);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] ProductStamp stamps)
        {
            if (ModelState.IsValid)
            {
                stamps.Status = 0;
                stamps.Createby = User.Identity.Name;
                stamps.Createdate = DateTime.Now;
                db.ProductStamps.Add(stamps);
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
            var stamps = db.ProductStamps.Find(Id);
            return PartialView("~/Areas/Admin/Views/ProductStamps/_Edit.cshtml", stamps);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] ProductStamp stamps)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stamps).State = EntityState.Modified;
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
                var stamps = db.ProductStamps.Find(Id);
                db.ProductStamps.Remove(stamps);
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
            List<ProductStamp> list_productstamps = new List<ProductStamp>();
            return View(list_productstamps);
        }
        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            listerror.Clear();
            List<ProductStamp> list_product = new List<ProductStamp>();
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
                            string serial;

                            try { code = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { code = ""; }
                            try { serial = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { serial = ""; }

                            var stamps = new ProductStamp()
                            {
                                Code = code,
                                Serial = serial,
                            };
                            //check trung 
                            if (!string.IsNullOrEmpty(code))
                            {
                                var _stamps = db.ProductStamps.Where(a => a.Code == code);
                                if (_stamps.Count() == 0)
                                {
                                    db.ProductStamps.Add(stamps);
                                    listerror.Add(stamps);
                                    db.SaveChanges();
                                }

                            }
                            stamps.Status = 0;
                            stamps.Createdate = DateTime.Now;
                            stamps.Createby = User.Identity.Name;
                            db.SaveChanges();
                            list_product.Add(stamps);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(list_product);
        }
        public void UploadFail()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Code lỗi";
            int index = 1;
            int row = 2;
            foreach (var item in listerror)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Code;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportUploadFail.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}