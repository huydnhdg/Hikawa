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
    public class PriceRepairController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        static List<Repair_Price> listerror = new List<Repair_Price>();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Repair_Price
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.Price.ToString().Contains(textsearch));
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
            IEnumerable<Repair_Price> data = model as IEnumerable<Repair_Price>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/PriceRepair/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Repair_Price repair)
        {
            if (ModelState.IsValid)
            {
                repair.Createdate = DateTime.Now;
                db.Repair_Price.Add(repair);
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
            var model = db.Repair_Price.Find(Id);
            return PartialView("~/Areas/Admin/Views/PriceRepair/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Repair_Price repair)
        {
            if (ModelState.IsValid)
            {
                db.Entry(repair).State = EntityState.Modified;
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
                var model = db.Repair_Price.Find(Id);
                db.Repair_Price.Remove(model);
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
            List<Repair_Price> list_product = new List<Repair_Price>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            listerror.Clear();
            List<Repair_Price> list_product = new List<Repair_Price>();
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
                            string capacity;
                            string name;
                            string price;
                            string des;

                            try { code = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { code = ""; }
                            try { capacity = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { capacity = ""; }
                            try { name = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { name = ""; }
                            try { price = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { price = ""; }
                            try { des = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { des = ""; }

                            //add thong tin rows vao product
                            var cate = new Repair_Price()
                            {
                                Code = code,
                                Capacity = capacity,
                                Name = name,
                                Price = int.Parse(price),
                                Description = des,
                                Createdate = DateTime.Now
                            };
                            //check trung
                            //if (!string.IsNullOrEmpty(code))
                            //{
                            //    var _cate = db.Repair_Price.Where(a => a.Code == code);
                            //    if (_cate.Count() == 0)
                            //    {
                            //        db.Repair_Price.Add(cate);
                            //        listerror.Add(cate);
                            //        db.SaveChanges();
                            //    }

                            //}
                            list_product.Add(cate);
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
        //public void UploadFail()
        //{
        //    ExcelPackage Ep = new ExcelPackage();
        //    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
        //    Sheet.Cells["A1"].Value = "STT";
        //    Sheet.Cells["B1"].Value = "Mã lỗi";
        //    int index = 1;
        //    int row = 2;
        //    foreach (var item in listerror)
        //    {
        //        Sheet.Cells[string.Format("A{0}", row)].Value = index++;
        //        Sheet.Cells[string.Format("B{0}", row)].Value = item.Code;
        //        row++;
        //    }
        //    Sheet.Cells["A:AZ"].AutoFitColumns();
        //    Response.Clear();
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.AddHeader("content-disposition", "attachment: filename=" + "ReportUploadFail.xlsx");
        //    Response.BinaryWrite(Ep.GetAsByteArray());
        //    Response.End();
        //}
    }
}