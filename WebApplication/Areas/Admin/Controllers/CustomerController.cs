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
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class CustomerController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        static IEnumerable<CustomerView> listexc = null;
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var model = from a in db.Customers
                        //join b in db.Products on a.Phone equals b.Active_phone into temp
                        //from t in temp.DefaultIfEmpty()
                        select new CustomerView()
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Phone = a.Phone,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Province = a.Province,
                            District = a.District,
                            Ward = a.Ward,
                            Createdate = a.Createdate,
                            Chanel = a.Chanel,
                            CountActive = db.Products.Where(c => c.Active_phone == a.Phone).Count(),
                            CountWarranti = db.Warrantis.Where(w => w.Phone == a.Phone).Count(),
                            Agent = db.Products.Where(g=>g.Active_phone == a.Phone).FirstOrDefault().Active_by,
                            Note = a.Note,
                            PointActive = a.PointActive,
                            PointPayment = a.PointPayment
                        };
            //hiển thị theo đại lý
            if (User.IsInRole("Đại lý") || User.IsInRole("Nhân viên "))
            {
                model = model.Where(a => a.Agent == User.Identity.Name);
            }

            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Name.Contains(textsearch)
                || a.Phone.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.Province.Contains(textsearch)
                || a.District.Contains(textsearch)
                || a.Ward.Contains(textsearch)
                || a.Note.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Chanel.Contains(chanel));
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(status))
            {
                if (status.Equals("chưa liên hệ"))
                {
                    model = model.Where(a => a.Calldate == null);
                    ViewBag.status = status;
                }
                else
                {
                    model = model.Where(a => a.Calldate != null);
                    ViewBag.status = status;
                }
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
            listexc = model as IEnumerable<CustomerView>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Customer/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                //check trung sdt
                var _cus = db.Customers.SingleOrDefault(a => a.Phone == customer.Phone);
                if (_cus == null)
                {
                    if (Utils.Control.getMobileOperator(customer.Phone) == "UNKNOWN")
                    {
                        SetAlert("Số điện thoại không đúng", "danger");
                        return RedirectToAction("Index");
                    }
                    customer.Chanel = "CMS";
                    customer.Createdate = DateTime.Now;
                    customer.Calldate = DateTime.Now;
                    customer.Callby = User.Identity.Name;
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    SetAlert("Đã lưu thông tin thành công", "success");
                }
                else
                {
                    SetAlert("Khách hàng này đã tồn tại trong hệ thống.", "danger");
                }

                return RedirectToAction("Index");

            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Customers.Find(Id);
            return PartialView("~/Areas/Admin/Views/Customer/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (Utils.Control.getMobileOperator(customer.Phone) == "UNKNOWN")
                {
                    SetAlert("Số điện thoại không đúng", "danger");
                    return RedirectToAction("Index");
                }
                var model = db.Customers.Find(customer.Id);
                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Edit] @UserName={0} @Customer={1}", User.Identity.Name, json));

                model.Name = customer.Name;
                model.Birthday = customer.Birthday;
                model.Email = customer.Email;
                model.Province = customer.Province;
                model.District = customer.District;
                model.Ward = customer.Ward;
                model.Address = customer.Address;
                model.Note = customer.Note;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        public void Khachhang()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "stt";
            Sheet.Cells["B1"].Value = "họ tên";
            Sheet.Cells["C1"].Value = "số điện thoại";
            Sheet.Cells["D1"].Value = "địa chỉ";
            Sheet.Cells["E1"].Value = "ngày sinh";
            Sheet.Cells["F1"].Value = "email";
            Sheet.Cells["G1"].Value = "ngày tạo";
            Sheet.Cells["H1"].Value = "kênh";
            Sheet.Cells["I1"].Value = "kích hoạt";
            Sheet.Cells["K1"].Value = "bảo hành";

            int index = 1;
            int row = 2;
            foreach (var item in listexc)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Phone;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("E{0}", row)].Value = (item.Birthday != null) ? item.Birthday.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("G{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Chanel;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.CountActive;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.CountWarranti;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult UploadFile()
        {
            List<Customer> list_product = new List<Customer>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            List<Customer> list_product = new List<Customer>();
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
                            string phone;
                            string birth;
                            string email;
                            string province;
                            string district;
                            string ward;
                            string address;

                            try { name = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { name = ""; }
                            try { phone = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { phone = ""; }
                            try { birth = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { birth = ""; }
                            try { email = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { email = ""; }
                            try { province = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { province = ""; }
                            try { district = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { district = ""; }
                            try { ward = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { ward = ""; }
                            try { address = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { address = ""; }


                            DateTime? date = null;
                            if (!string.IsNullOrEmpty(birth))
                            {
                                date = DateTime.ParseExact(birth, "dd/MM/yyyy", null);
                            }
                            //add thong tin rows vao product
                            var cate = new Customer()
                            {
                                Name = name,
                                Phone = phone,
                                Birthday = date,
                                Email = email,
                                Province = province,
                                District = district,
                                Ward = ward,
                                Address = address
                            };
                            //check trung serial code
                            if (!string.IsNullOrEmpty(phone) && Utils.Control.getMobileOperator(phone) != "UNKNOWN")
                            {
                                var _cate = db.Customers.Where(a => a.Phone == phone);
                                if (_cate.Count() == 0)
                                {
                                    db.Customers.Add(cate);
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