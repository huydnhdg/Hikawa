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
    public class PriceModelController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Model_Price
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Model.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            
            IEnumerable<Model_Price> data = model as IEnumerable<Model_Price>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Model).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/PriceModel/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Model_Price model_Price)
        {
            if (ModelState.IsValid)
            {
                model_Price.Createdate = DateTime.Now;
                db.Model_Price.Add(model_Price);
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
            var model_Price = db.Model_Price.Find(Id);
            return PartialView("~/Areas/Admin/Views/PriceModel/_Edit.cshtml", model_Price);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Model_Price model_Price)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model_Price).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        public ActionResult UploadFile()
        {
            List<Model_Price> list_product = new List<Model_Price>();
            return View(list_product);
        }
        [HttpPost]
        public ActionResult GetModel(string text)
        {
            
            var cate = (from a in db.Products
                        group a by a.Model into g
                        where g.FirstOrDefault().Model.Contains(text)
                        select new { g.FirstOrDefault().Model });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Model_Price> list_product = new List<Model_Price>();
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
                            string name;
                            string price;

                            try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }
                            try { price = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { price = ""; }

                            //add thong tin rows vao product
                            var cate = new Model_Price()
                            {
                                Model = name,
                                Price = int.Parse(price),
                                Createdate = DateTime.Now
                            };
                            //check trung serial code
                            if (!string.IsNullOrEmpty(name))
                            {
                                var _cate = db.Model_Price.Where(a => a.Model == name);
                                if (_cate.Count() == 0)
                                {
                                    db.Model_Price.Add(cate);
                                    db.SaveChanges();
                                }

                            }
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