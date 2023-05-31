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
    public class ABigController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //ELMEntities db = new ELMEntities();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Accessary_Big
                        select new A_B_Model()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            CountExport = a.CountExport,
                            CountImport = a.CountImport,
                            Exist = a.CountImport - a.CountExport,
                            KeyPrice = a.KeyPrice,
                            Name = a.Name,
                            ProductName = a.ProductName,
                            Discount = a.Discount,
                            Model = a.Model
                        };
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.ProductName.Contains(textsearch)
                || a.KeyPrice.ToString().Contains(textsearch)
                || a.Discount.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            IEnumerable<A_B_Model> data = model as IEnumerable<A_B_Model>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderBy(a => a.Name).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/ABig/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Accessary_Big accessary)
        {
            if (ModelState.IsValid)
            {
                db.Accessary_Big.Add(accessary);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Liên hệ ngày quản trị hệ thống", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Accessary_Big.Find(Id);
            return PartialView("~/Areas/Admin/Views/ABig/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Accessary_Big accessary)
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

        public ActionResult UploadFile()
        {
            List<Accessary_Import> list_product = new List<Accessary_Import>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Accessary_Import> list_product = new List<Accessary_Import>();
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
                            string cimport;
                            string code;
                            string name;
                            string cate;
                            string quantity;
                            string price;
                            string note;
                            string model;

                            try { cimport = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { cimport = ""; }
                            try { code = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { code = ""; }
                            try { name = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { name = ""; }
                            try { cate = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { cate = ""; }
                            try { model = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { model = ""; }
                            try { quantity = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { quantity = ""; }
                            try { price = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { price = ""; }
                            try { note = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { note = ""; }


                            var import = new Accessary_Import()
                            {
                                CodeImport = cimport,
                                Code = code,
                                Name = name,
                                ProductName = cate,
                                Quantity = int.Parse(quantity),
                                Price = int.Parse(price),
                                Amount = int.Parse(quantity) * int.Parse(price),
                                Createdate = DateTime.Now,
                                Note = note,
                                Model = model
                            };
                            db.Accessary_Import.Add(import);

                            var item = db.Accessary_Big.SingleOrDefault(a => a.Code == import.Code);
                            if (item != null)
                            {
                                //update số lượng tổng nhập
                                item.CountImport = item.CountImport + import.Quantity;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                //tạo mới linh kiện,
                                var a_big = new Accessary_Big()
                                {
                                    Code = code,
                                    Name = name,
                                    ProductName = cate,
                                    KeyPrice = import.Price,
                                    CountImport = import.Quantity,
                                    Model = import.Model
                                };
                                db.Accessary_Big.Add(a_big);
                                db.SaveChanges();
                            }
                            //add list item vao view
                            list_product.Add(import);
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

        //Update mới xuất linh kiện bằng file exel
        public ActionResult UploadAExport()
        {
            List<Acessary_Export_Item> list_product = new List<Acessary_Export_Item>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadAExport(FormCollection collection)//Bổ sung ListKey trong A_Export_Model)
        {
            List<Acessary_Export_Item> list_product = new List<Acessary_Export_Item>();
            try
            {
                int ERROR = 0;
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
                            string export_id;//lấy id phiếu
                            string user_key;//lấy id trạm
                            string code;
                            string price;
                            string quantity;
                            string name;
                            string model;
                            string cate;
                            string orderdate;
                            string note;

                            //lấy id phiếu
                            try { export_id = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { export_id = ""; }
                            //lấy id trạm    
                            try { user_key = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { user_key = ""; }
                            try { code = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { code = ""; }
                            try { price = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { price = ""; }
                            try { quantity = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { quantity = ""; }
                            try { name = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { name = ""; }
                            try { model = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { model = ""; }
                            try { cate = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { cate = ""; }
                            try { orderdate = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { orderdate = ""; }
                            try { note = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { note = ""; }
                            //lưu thông tin trạm (Nhưng chưa check key_id có tồn tại không)
                            //Tạo phiếu xuất đây
                            //kiểm tra tài khoản trạm
                            var currentUser = db.AspNetUsers.FirstOrDefault(a => a.UserName == user_key);
                            if (currentUser == null)
                            {
                                ERROR++;
                                SetAlert("Trạm không tồn tại.", "danger");
                                continue;
                            }
                            //kiểm tra mã phiếu// trung ma cu
                            string _code = export_id.ToUpper();
                            var export = new Accessary_Export();
                            var currentExport = db.Accessary_Export.FirstOrDefault(a => a.Code == _code);
                            if (currentExport != null)
                            {
                                export = currentExport;
                            }
                            else
                            {
                                var n_export = new Accessary_Export()
                                {
                                    Id_Key = user_key,
                                    Code = _code,
                                    Createdate = DateTime.Now,
                                    Note = note,
                                };
                                db.Accessary_Export.Add(n_export);
                                db.SaveChanges();
                                export = n_export;
                            }

                            int pri = int.Parse(price);
                            int qty = int.Parse(quantity);
                            DateTime? date = null;
                            if (!string.IsNullOrEmpty(orderdate))
                            {
                                try
                                {
                                    date = DateTime.ParseExact(orderdate, "dd/MM/yyyy", null);
                                }
                                catch (Exception ex)
                                {
                                    date = DateTime.Parse(orderdate);
                                }

                            }
                            var items = new Acessary_Export_ItemView()
                            {
                                Id_Export = export.Id,
                                Code_Export = export_id,
                                UserKey = user_key,
                                Code = code,
                                Price = int.Parse(price),
                                Quantity = int.Parse(quantity),
                                Amount = pri * qty,
                                Name = name,
                                Model = model,
                                Cate = cate,
                                Orderdate = date,
                                Note = note,
                            };
                            //đang lỗi đoạn này đúng k anh debug lại đi
                            db.Acessary_Export_Item.Add(items);
                            db.SaveChanges();
                            logger.Error("log--" + items.Id);
                            list_product.Add(items);

                            var big_acc = db.Accessary_Big.SingleOrDefault(a => a.Code == items.Code);
                            if (big_acc != null && (big_acc.CountImport - big_acc.CountExport > 0))
                            {
                                //cộng linh kiện cho trạm
                                string idkey = "";
                                var _old = db.Accessary_Key.Where(a => a.Id_Key == items.UserKey).SingleOrDefault(a => a.Code == items.Code);
                                if (_old != null)
                                {
                                    big_acc.CountExport = big_acc.CountExport + items.Quantity;
                                    db.Entry(big_acc).State = EntityState.Modified;
                                    //da co linh kien nay update thoi
                                    _old.CountImport = _old.CountImport + items.Quantity;
                                    db.Entry(_old).State = EntityState.Modified;

                                    idkey = _old.Id;
                                }
                                else
                                {
                                    big_acc.CountExport = big_acc.CountExport + items.Quantity;
                                    db.Entry(big_acc).State = EntityState.Modified;
                                    //chua co linh kien nay add new
                                    var a_key = new Accessary_Key()
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        Name = big_acc.Name,
                                        Code = big_acc.Code,
                                        ProductName = big_acc.ProductName,
                                        KeyPrice = big_acc.KeyPrice,
                                        CountImport = items.Quantity,
                                        Id_Key = items.UserKey,
                                    };
                                    db.Accessary_Key.Add(a_key);
                                    idkey = a_key.Id;
                                }

                                var log_akey = new Acessary_Log_Key()
                                {
                                    Accessary = items.Code,
                                    Code = export.Code,
                                    Count = items.Quantity,
                                    Createdate = DateTime.Now,
                                    Id_Akey = idkey,
                                    Type = 1//log nhap
                                };
                                db.Acessary_Log_Key.Add(log_akey);
                                db.SaveChanges();
                            }
                            else
                            {
                                ERROR++;
                                SetAlert("Mã linh kiện này không còn trong kho.", "danger");
                                break;
                            }
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
        //End Update mới xuất linh kiện bằng file exel

        public ActionResult Export()
        {
            var model = new A_Export_Model()
            {
                ListItem = new List<Acessary_Export_Item>()
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Export([Bind(Include = "")] A_Export_Model a_Export_Model)
        {
            int ERROR = 0;
            long ID = 0;
            string msg = "";
            try
            {
                //tạo phiếu xuất hàng
                var export = new Accessary_Export()
                {
                    Id_Key = a_Export_Model.Id_Key,
                    Createdate = DateTime.Now,
                    Note = a_Export_Model.Note
                };
                db.Accessary_Export.Add(export);
                db.SaveChanges();
                export.Code = Utils.Control.CreateAExportCode(export.Id);
                db.Entry(export).State = EntityState.Modified;
                //lưu items cho phiếu
                if (a_Export_Model.ListItem.Count > 0)
                {
                    foreach (var item in a_Export_Model.ListItem)
                    {
                        if (!string.IsNullOrEmpty(item.Code))
                        {
                            var big_acc = db.Accessary_Big.SingleOrDefault(a => a.Code == item.Code);

                            var itemmodel = new Acessary_Export_Item()
                            {
                                Id_Export = export.Id,
                                Code = item.Code,
                                Price = item.Price,
                                Quantity = item.Quantity,
                                Amount = item.Amount,
                                Name = item.Name,
                                Model = item.Model,
                                Cate = item.Cate,
                                Orderdate = item.Orderdate,
                                Note = item.Note
                            };
                            db.Acessary_Export_Item.Add(itemmodel);
                            //cộng linh kiện cho trạm
                            string idkey = "";
                            var _old = db.Accessary_Key.Where(a => a.Id_Key == a_Export_Model.Id_Key).SingleOrDefault(a => a.Code == item.Code);
                            if (_old != null)
                            {
                                big_acc.CountExport = big_acc.CountExport + item.Quantity;
                                db.Entry(big_acc).State = EntityState.Modified;
                                //da co linh kien nay update thoi
                                _old.CountImport = _old.CountImport + item.Quantity;
                                db.Entry(_old).State = EntityState.Modified;

                                idkey = _old.Id;
                            }
                            else
                            {
                                big_acc.CountExport = big_acc.CountExport + item.Quantity;
                                db.Entry(big_acc).State = EntityState.Modified;
                                //chua co linh kien nay add new
                                var a_key = new Accessary_Key()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Name = big_acc.Name,
                                    Code = big_acc.Code,
                                    ProductName = big_acc.ProductName,
                                    KeyPrice = big_acc.KeyPrice,
                                    CountImport = item.Quantity,
                                    Id_Key = a_Export_Model.Id_Key
                                };
                                db.Accessary_Key.Add(a_key);
                                idkey = a_key.Id;
                            }

                            var log_akey = new Acessary_Log_Key()
                            {
                                Accessary = itemmodel.Code,
                                Code = export.Code,
                                Count = itemmodel.Quantity,
                                Createdate = DateTime.Now,
                                Id_Akey = idkey,
                                Type = 1//log nhap
                            };
                            db.Acessary_Log_Key.Add(log_akey);

                        }
                        else
                        {
                            ERROR++;
                            msg = "Không xác định được mã linh kiện.";
                            break;
                        }
                    }
                    if (ERROR == 0)
                    {
                        db.SaveChanges();
                        SetAlert("Đã lưu thông tin thành công.", "success");
                        return RedirectToAction("Index", "AExport");
                    }
                }
                else
                {
                    msg = "Không có linh kiện nào được chọn trong phiếu xuất.";
                }
            }
            catch (Exception ex)
            {
                //xoa phieu khong luu linh kien thanh cong
                if (ID > 0)
                {
                    var model = db.Accessary_Export.Find(ID);
                    db.Accessary_Export.Remove(model);
                    db.SaveChanges();
                }
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
                SetAlert(ex.Message, "danger");
                return View(a_Export_Model);
            }
            SetAlert(msg, "danger");
            return View(a_Export_Model);
        }

        [HttpPost]
        public ActionResult GetPriceAccess(string name)
        {
            var product = db.Accessary_Big.Where(a => a.Code == name || a.Name == name).SingleOrDefault();
            return Json(product.KeyPrice, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAccessary(string text)
        {
            var cate = (from a in db.Accessary_Big
                        where a.Code.Contains(text) || a.Name.Contains(text)
                        select new { a.Code, a.Name, a.Model });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
    }
}