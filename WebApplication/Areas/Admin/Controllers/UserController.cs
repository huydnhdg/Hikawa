using ImageResizer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using NLog;
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
using WebApplication.APIFORAPP.Model_Teka.Model_Request;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        
        static IEnumerable<UserView> listexc = null;
        static List<AspNetUser> listerror = new List<AspNetUser>();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        //where b.Id != "Customer"
                        select new UserView()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Role = b.Name,
                            FullName = a.FullName,
                            StationHead = a.StationHead,
                            StatusConfirm = a.StatusConfirm,
                            Fee = a.Fee,
                            Company = a.Company,
                            Tax = a.Tax,
                            Bank = a.Bank,
                            BankNumber = a.BankNumber,
                            InvoiceType = a.InvoiceType
                        };
            //hiển thị theo quyền
            if (User.IsInRole("Trạm - Trưởng trạm"))
            {
                model = model.Where(a => a.Createby == User.Identity.Name || a.UserName == User.Identity.Name);
            }
            //đại lý, thợ chỉ hiển thị tài khoản cá nhân
            if (User.IsInRole("Trạm - Nhân viên kỹ thuật") || User.IsInRole("Đại lý") || User.IsInRole("Nhân viên") || User.IsInRole("Thủ kho"))
            {
                model = model.Where(a => a.UserName == User.Identity.Name);
            }
            //lọc theo thông tin 
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.UserName.Contains(textsearch)
                || a.PhoneNumber.Contains(textsearch)
                || a.Email.Contains(textsearch)
                || a.Address.Contains(textsearch)
                || a.FullName.Contains(textsearch)
                || a.Role.ToString().Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Role.Contains(chanel));
                ViewBag.chanel = chanel;
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
            ViewBag.role = db.AspNetRoles.ToList();
            listexc = model as IEnumerable<UserView>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(listexc.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        ApplicationDbContext context = new ApplicationDbContext();
        [HttpPost]
        public ActionResult Create()
        {
            if (User.IsInRole("Admin - Quản trị toàn hệ thống"))
            {
                ViewBag.role = context.Roles.ToList();
            }
            else if(User.IsInRole("Trạm - Trưởng trạm"))
            {
                ViewBag.role = context.Roles.Where(a => a.Id == "KTV").ToList();
            }
            else if(User.IsInRole("Đại lý"))
            {
                ViewBag.role = context.Roles.Where(a => a.Id == "Staff").ToList();
            }
            return PartialView("~/Areas/Admin/Views/User/_Create.cshtml", null);
        }

        [HttpPost]
        public JsonResult GetProvince()
        {
            var province = (from a in db.Provinces
                            orderby a.Name
                            select new { a.Name });
            return Json(province, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDistrict(string text)
        {
            var district = (from a in db.Districts
                            orderby a.Name
                            where a.Province.Name.Equals(text)
                            select new { a.Name });
            return Json(district, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetWard(string text)
        {
            var ward = (from a in db.Wards
                        orderby a.Name
                        where a.District.Name.Equals(text)
                        select new { a.Name });
            return Json(ward, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(string Id)
        {
            ViewBag.role = context.Roles.ToList();
            var model = db.AspNetUsers.Find(Id);
            return PartialView("~/Areas/Admin/Views/User/_Edit.cshtml", model);
        }

        [HttpPost]
        public ActionResult EditConfirm([Bind(Include = "")] AspNetUser _user, string Roler, HttpPostedFileBase ImgIDFront, HttpPostedFileBase ImgIDBack)
        {
            if (ModelState.IsValid)
            {
                if (Utils.Control.getMobileOperator(_user.PhoneNumber) == "UNKNOWN")
                {
                    SetAlert("Số điện thoại không đúng", "danger");
                    return RedirectToAction("Index");
                }
                
                var user = db.AspNetUsers.Find(_user.Id);
                if (User.IsInRole("Admin - Quản trị toàn hệ thống"))
                {
                    //string json = JsonConvert.SerializeObject(user);
                    //logger.Info(string.Format("[Edit] @UserName={0} @User={1}", User.Identity.Name, json));
                    //update quyền cho tài khoản
                    string r = user.AspNetRoles.FirstOrDefault().Name;
                    if (!user.AspNetRoles.FirstOrDefault().Name.Equals(Roler))
                    {
                        ApplicationUser u = context.Users.Find(user.Id);
                        //xoa di roi add lai la duoc
                        u.Roles.Remove(u.Roles.FirstOrDefault());
                        u.Roles.Add(new IdentityUserRole() { UserId = user.Id, RoleId = Roler });
                        context.SaveChanges();
                    }
                }
                user.PhoneNumber = _user.PhoneNumber;
                user.Address = _user.Address;
                user.Ward = _user.Ward;
                user.District = _user.District;
                user.Province = _user.Province;
                user.Email = _user.Email;
                user.FullName = _user.FullName;
                user.StationHead = _user.StationHead;
                user.Fee = _user.Fee;
                user.Company = _user.Company;
                user.Tax = _user.Tax;
                user.Bank = _user.Bank;
                user.BankNumber = _user.BankNumber;
                user.InvoiceType = _user.InvoiceType;
                if (Roler == "Trạm - Nhân viên kỹ thuật")
                {
                    string strrand = Guid.NewGuid().ToString();
                    ResizeSettings resizeSetting = new ResizeSettings
                    {
                        MaxWidth = 300,
                        MaxHeight = 200,
                    };
                    if (ImgIDFront != null)
                    {
                        user.ImgIDFront = null;
                        //var oldImgFr = user.ImgIDFront;
                        //System.IO.File.Delete(oldImgFr);
                        var fileNameFr = System.IO.Path.GetFileName(ImgIDFront.FileName);
                        var pathImgFr = System.IO.Path.Combine(Server.MapPath("~/Data/ImageID/"), strrand + "_" + fileNameFr);
                        ImgIDFront.SaveAs(pathImgFr);
                        ImageBuilder.Current.Build(pathImgFr, pathImgFr, resizeSetting);
                        string linkImgFr = "/Data/ImageID/" + strrand + "_" + fileNameFr;
                        user.ImgIDFront = linkImgFr;
                    }
                    if(ImgIDBack != null)
                    {
                        user.ImgIDBack = null;
                        //var oldImgBa = user.ImgIDBack;
                        //System.IO.File.Delete(oldImgBa);
                        var fileNameBa = System.IO.Path.GetFileName(ImgIDBack.FileName);
                        var pathImgBa = System.IO.Path.Combine(Server.MapPath("~/Data/ImageID/"), strrand + "_" + fileNameBa);
                        ImgIDBack.SaveAs(pathImgBa);
                        ImageBuilder.Current.Build(pathImgBa, pathImgBa, resizeSetting);
                        string linkImgBa = "/Data/ImageID/" + strrand + "_" + fileNameBa;
                        user.ImgIDBack = linkImgBa;
                    }
                    //trạng thái chờ duyệt tài khoản set là 777
                    user.StatusConfirm = 777;
                }
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                ViewBag.role = context.Roles.ToList();
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            ViewBag.role = context.Roles.ToList();
            return RedirectToAction("Index");
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult UploadFile()
        {
            List<AspNetUser> list_product = new List<AspNetUser>();
            return View(list_product);
        }

        [HttpPost]
        public ActionResult UploadFile(FormCollection collection)
        {
            listerror.Clear();
            List<AspNetUser> list_product = new List<AspNetUser>();
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
                            string user;
                            string password;
                            string phone;
                            string address;
                            string ward;
                            string district;
                            string province;
                            string email;
                            string role;
                            string fullname;
                            string fee;
                            string company;
                            string tax;
                            string bank;
                            string banknumber;
                            string invoiceType;
                            string stationhead;

                            try { user = workSheet.Cells[rowIterator, 1].Value.ToString(); } catch (Exception) { user = ""; }
                            try { password = workSheet.Cells[rowIterator, 2].Value.ToString(); } catch (Exception) { password = ""; }
                            try { fullname = workSheet.Cells[rowIterator, 3].Value.ToString(); } catch (Exception) { fullname = ""; }
                            try { stationhead = workSheet.Cells[rowIterator, 4].Value.ToString(); } catch (Exception) { stationhead = ""; }
                            try { phone = workSheet.Cells[rowIterator, 5].Value.ToString(); } catch (Exception) { phone = ""; }
                            try { address = workSheet.Cells[rowIterator, 6].Value.ToString(); } catch (Exception) { address = ""; }
                            try { ward = workSheet.Cells[rowIterator, 7].Value.ToString(); } catch (Exception) { ward = ""; }
                            try { district = workSheet.Cells[rowIterator, 8].Value.ToString(); } catch (Exception) { district = ""; }
                            try { province = workSheet.Cells[rowIterator, 9].Value.ToString(); } catch (Exception) { province = ""; }

                            try { email = workSheet.Cells[rowIterator, 10].Value.ToString(); } catch (Exception) { email = ""; }
                            try { fee = workSheet.Cells[rowIterator, 11].Value.ToString(); } catch (Exception) { fee = ""; }
                            try { company = workSheet.Cells[rowIterator, 12].Value.ToString(); } catch (Exception) { company = ""; }
                            try { tax = workSheet.Cells[rowIterator, 13].Value.ToString(); } catch (Exception) { tax = ""; }
                            try { bank = workSheet.Cells[rowIterator, 14].Value.ToString(); } catch (Exception) { bank = ""; }
                            try { banknumber = workSheet.Cells[rowIterator, 15].Value.ToString(); } catch (Exception) { banknumber = ""; }
                            try { invoiceType = workSheet.Cells[rowIterator, 16].Value.ToString(); } catch (Exception) { invoiceType = ""; }
                            try { role = workSheet.Cells[rowIterator, 17].Value.ToString(); } catch (Exception) { role = ""; }
                            
                            
                            //add thong tin rows vao product
                            var product = new AspNetUser()
                            {
                                UserName = user,
                                Address = address + " " + ward + " " + district + " " + province,
                                PhoneNumber = phone,
                                Email = email,
                                FullName = fullname,
                                Fee = fee,
                                Company = company,
                                Tax = tax,
                                Bank = bank,
                                BankNumber = banknumber,
                                InvoiceType = invoiceType,
                                StationHead = stationhead
                            };
                            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(role))
                            {
                                var drole = db.AspNetRoles.Where(a => a.Id == role);
                                if (drole.Count() == 0)
                                {
                                    listerror.Add(product);
                                    //product.FullName = "phân quyền k đúng";
                                }
                                //else if (phone.Length != 10)
                                //{
                                //    listerror.Add(product);
                                //    //product.PhoneNumber = "sđt không đúng";
                                //}
                                else
                                {
                                    //tao tai khoan
                                    try
                                    {
                                        var asp = new ApplicationUser
                                        {
                                            UserName = user,
                                            PhoneNumber = phone,
                                            Email = email
                                        };

                                        var result = UserManager.Create(asp, password);
                                        if (result.Succeeded)
                                        {
                                            //add quyền cho tài khoản luôn
                                            ApplicationUser u = context.Users.Find(asp.Id);
                                            u.Roles.Add(new IdentityUserRole() { UserId = asp.Id, RoleId = role });
                                            context.SaveChanges();

                                            //bổ sung thong tin tài khoản
                                            var _user = db.AspNetUsers.Find(asp.Id);
                                            _user.Address = address;
                                            _user.Ward = ward;
                                            _user.District = district;
                                            _user.Province = province;
                                            _user.Createby = User.Identity.Name;
                                            _user.Createdate = DateTime.Now;
                                            _user.FullName = fullname;
                                            _user.Fee = fee;
                                            _user.Company = company;
                                            _user.Tax = tax;
                                            _user.Bank = bank;
                                            _user.BankNumber = banknumber;
                                            _user.InvoiceType = invoiceType;
                                            _user.StationHead = stationhead;
                                            db.Entry(_user).State = EntityState.Modified;
                                            db.SaveChanges();
                                            product.Id = asp.Id;
                                        }
                                        else
                                        {
                                            listerror.Add(product);
                                            //product.FullName = "không tạo được tài khoản";
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        product.FullName = ex.Message;
                                        logger.Error(ex.Message);
                                        logger.Error(ex.InnerException);
                                    }
                                }


                            }
                            listerror.Add(product);
                            list_product.Add(product);
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
        
        public ActionResult ConfirmAccount(string Id)
        {
            var model = db.AspNetUsers.Find(Id);
            return PartialView("~/Areas/Admin/Views/User/ConfirmAccount.cshtml", model);
        }
        [HttpPost]
        public ActionResult ConfirmAccount([Bind(Include = "")] AspNetUser _user)
        {
            try
            {
                var model = db.AspNetUsers.Find(_user.Id);
                model.StatusConfirm = null;
                db.SaveChanges();
                SetAlert("Duyệt tài khoản thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }
            ViewBag.role = db.AspNetRoles.ToList();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(string Id)
        {
            try
            {
                var model = db.AspNetUsers.Find(Id);
                db.AspNetUsers.Remove(model);
                db.SaveChanges();
                string json = JsonConvert.SerializeObject(model);
                logger.Info(string.Format("[Delete] @UserName={0} @User={1}", User.Identity.Name, json));
                SetAlert("Xóa tài khoản thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }
            ViewBag.role = db.AspNetRoles.ToList();
            return RedirectToAction("Index");
        }
        public void UploadFail()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Tài khoản lỗi";
            int index = 1;
            int row = 2;
            foreach (var item in listerror)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReportUploadFail.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public void Export_User()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Tài khoản đại lý";
            Sheet.Cells["C1"].Value = "Tên đại lý";
            Sheet.Cells["D1"].Value = "Trưởng trạm";
            Sheet.Cells["E1"].Value = "Quyền";
            Sheet.Cells["F1"].Value = "Số điện thoại";
            Sheet.Cells["G1"].Value = "Email";
            Sheet.Cells["H1"].Value = "Địa chỉ";
            Sheet.Cells["I1"].Value = "Ngày tạo";
            Sheet.Cells["J1"].Value = "Phí cố định";
            Sheet.Cells["K1"].Value = "Công ty";
            Sheet.Cells["L1"].Value = "Mã số thuế";
            Sheet.Cells["M1"].Value = "Ngân hàng";
            Sheet.Cells["N1"].Value = "STK";
            Sheet.Cells["O1"].Value = "Loại hoá đơn";

            int index = 1;
            int row = 2;
            foreach (var item in listexc)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FullName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.StationHead;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Role;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.PhoneNumber;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Email;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("I{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Fee;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.Company;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Tax;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.Bank;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.BankNumber;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.InvoiceType;

                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}