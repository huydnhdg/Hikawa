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
    public class PriceServiceController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        // GET: Admin/PriceService
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Service_Price
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
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
            IEnumerable<Service_Price> data = model as IEnumerable<Service_Price>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/PriceService/_Create.cshtml", null);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Service_Price service)
        {
            if (ModelState.IsValid)
            {
                service.Createdate = DateTime.Now;
                db.Service_Price.Add(service);
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
            var model = db.Service_Price.Find(Id);
            return PartialView("~/Areas/Admin/Views/PriceService/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Service_Price service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
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
                var model = db.Service_Price.Find(Id);
                db.Service_Price.Remove(model);
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
            List<Service_Price> list_product = new List<Service_Price>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Service_Price> list_product = new List<Service_Price>();
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
                            string capacity;
                            string name;
                            string price;

                            try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }
                            try { capacity = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { capacity = ""; }
                            try { price = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { price = ""; }

                            //add thong tin rows vao product
                            var cate = new Service_Price()
                            {
                                Capacity = capacity,
                                Name = name,
                                Price = int.Parse(price),
                                Createdate = DateTime.Now
                            };
                            //check trung serial code
                            //if (!string.IsNullOrEmpty(name))
                            //{
                            //    var _cate = db.Service_Price.Where(a => a.Name == name);
                            //    if (_cate.Count() == 0)
                            //    {
                            //        db.Service_Price.Add(cate);
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
    }
}