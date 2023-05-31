using Newtonsoft.Json;
using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class B_ModelController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.B_Model
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Model.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            IEnumerable<B_Model> data = model as IEnumerable<B_Model>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Model).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            var list = from a in db.B_Process
                       where a.Status == true
                       select new MapProcessModel()
                       {
                           Id = a.Id,
                           IdProcess = a.Name,
                           Status = false,
                           Name = a.Name,
                           Description = a.Description
                       };
            var model = new B_Model_List();
            model.Data = list.ToList();
            return PartialView("~/Areas/Admin/Views/B_Model/_Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] B_Model_List bmodel)
        {
            if (ModelState.IsValid)
            {
                var model = db.B_Model.Where(a => a.Model == bmodel.Model);
                if (model.Count() > 0)
                {
                    SetAlert("Model này đã được cấu hình trước đó. Hãy kiểm tra lại.", "danger");
                    return RedirectToAction("Index");
                }
                var _model = new B_Model()
                {
                    Model = bmodel.Model,
                    Iframe = bmodel.Iframe,
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name
                };
                db.B_Model.Add(_model);
                foreach (var item in bmodel.Data)
                {
                    var _process = new B_Model_Process()
                    {
                        Process = item.Id,
                        Model = bmodel.Model,
                        Status = item.Status,
                        Createdate = DateTime.Now,
                        Createby = User.Identity.Name
                    };
                    db.B_Model_Process.Add(_process);
                }

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
            var _model = db.B_Model.Find(Id);

            //var list = from a in db.B_Model_Process
            //           where a.Model == _model.Model
            //           join b in db.B_Process on a.Process equals b.Name into temp
            //           from t in temp.DefaultIfEmpty()
            //           select new MapProcessModel()
            //           {
            //               Id = a.Id,
            //               IdProcess = a.Process,
            //               Status = a.Status,
            //               Name = t.Name,
            //               Description = t.Description
            //           };
            var list = from a in db.B_Process
                       where a.Status == true
                       join b in db.B_Model_Process on a.Id equals b.Process into temp
                       from t in temp.DefaultIfEmpty()
                       where t.Model == _model.Model
                       select new MapProcessModel()
                       {
                           IdProcess = a.Name,
                           Status = (t != null) ? t.Status : false,
                           Name = a.Name,
                           Description = a.Description
                       };
            var model = new B_Model_List();
            model.Id = _model.Id;
            model.Model = _model.Model;
            model.Iframe = _model.Iframe;
            model.Data = list.ToList();
            return PartialView("~/Areas/Admin/Views/B_Model/_Edit.cshtml", model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] B_Model_List bmodel)
        {
            if (ModelState.IsValid)
            {
                var _model = db.B_Model.Find(bmodel.Id);
                _model.Model = bmodel.Model;
                _model.Iframe = bmodel.Iframe;
                db.Entry(_model).State = EntityState.Modified;
                foreach (var item in bmodel.Data)
                {
                    if (item.Id > 0)
                    {
                        var _process = db.B_Model_Process.Find(item.Id);
                        _process.Status = item.Status;
                        db.Entry(_process).State = EntityState.Modified;
                    }
                    else
                    {
                        var _process = new B_Model_Process()
                        {
                            Process = item.Id,
                            Model = bmodel.Model,
                            Status = item.Status,
                            Createdate = DateTime.Now,
                            Createby = User.Identity.Name
                        };
                        db.B_Model_Process.Add(_process);
                    }
                }
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
                var model = db.B_Model.Find(Id);

                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @Model={1}", User.Identity.Name, json));

                db.B_Model.Remove(model);


                var process = db.B_Model_Process.Where(a => a.Model == model.Model);
                if (process.Count() > 0)
                {
                    db.B_Model_Process.RemoveRange(process);
                }
                db.SaveChanges();
                SetAlert("Đã xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult GetModel(string text)
        {
            var cate = (from a in db.Products
                        where a.Model.Contains(text)
                        group a by a.Model into g
                        select new { Model = g.FirstOrDefault().Model });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadFile()
        {
            List<B_Model> list_product = new List<B_Model>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<B_Model> list_product = new List<B_Model>();
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
                            string model;
                            string iframe;
                            string warranti;
                            string savepoint;

                            try { model = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { model = ""; }
                            try { iframe = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { iframe = ""; }
                            try { warranti = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { warranti = ""; }
                            try { savepoint = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { savepoint = ""; }

                            //add thong tin rows vao product
                            var _model = new B_Model()
                            {
                                Model = model,
                                Iframe = iframe,
                                Createdate = DateTime.Now,
                                Createby = User.Identity.Name
                            };
                            if (!string.IsNullOrEmpty(model))
                            {
                                
                                var _check = db.B_Model.FirstOrDefault(a => a.Model == model);
                                //neu chua co thi tao moi
                                if (_check == null)
                                {
                                    db.B_Model.Add(_model);
                                    addProcess(warranti, savepoint, model);
                                }
                                //neu co roi thi check xem add chuong trinh chua
                                else
                                {
                                    _model.Id = _check.Id;
                                    addProcess(warranti, savepoint, model);
                                }
                                db.SaveChanges();
                            }

                            list_product.Add(_model);
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
        public void addProcess( string warranti, string savepoint, string model)
        {
            var process = db.B_Process;
            var CBH = process.FirstOrDefault(a => a.CODE == warranti);
            var pro_bh = db.B_Model_Process.Where(a => a.Process == CBH.Id && a.Model == model);
            if (CBH != null && pro_bh.Count() == 0)
            {
                var _process_bh = new B_Model_Process()
                {
                    Process = CBH.Id,
                    Model = model,
                    Status = CBH.Status,
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name
                };
                db.B_Model_Process.Add(_process_bh);
            }

            var TD = process.FirstOrDefault(a => a.CODE == savepoint);
            var pro_td = db.B_Model_Process.Where(a => a.Process == TD.Id && a.Model == model);
            if (TD != null && pro_td.Count() == 0)
            {
                var _process_td = new B_Model_Process()
                {
                    Process = TD.Id,
                    Model = model,
                    Status = TD.Status,
                    Createdate = DateTime.Now,
                    Createby = User.Identity.Name
                };
                db.B_Model_Process.Add(_process_td);
            }

        }
    }
}