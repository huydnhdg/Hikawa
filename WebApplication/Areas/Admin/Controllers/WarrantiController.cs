using ImageResizer;
using NLog;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication.Areas.Admin.Data;
using WebApplication.FCM;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Areas.Admin.Controllers
{
    public class WarrantiController : BaseController
    {

        Logger logger = LogManager.GetCurrentClassLogger();
        static IEnumerable<WarrantiDetail> listexc = null;
        public ActionResult Index(int? page, string phone, string textsearch, string station, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            string UserName = User.Identity.Name;
            var model = from a in db.Warrantis
                        join cus in db.Customers on a.Phone equals cus.Phone into tempc
                        from c in tempc.DefaultIfEmpty()
                            //join b in db.Products on a.ProductCode equals b.Code into temp
                            //from p in temp.DefaultIfEmpty()

                        join s in db.AspNetUsers on a.Station_Warranti equals s.UserName into temps
                        from st in temps.DefaultIfEmpty()
                        join k in db.AspNetUsers on a.KTV_Warranti equals k.UserName into tempk
                        from kt in tempk.DefaultIfEmpty()


                        select new WarrantiDetail()
                        {
                            Id = a.Id,
                            Status = a.Status,
                            Code = a.Code,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Chanel = a.Chanel,
                            Phone = a.Phone,
                            PhoneExtra = a.PhoneExtra,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                            Cate = a.Cate,
                            Warranti_Cate = a.Warranti_Cate,
                            Note = a.Note,
                            Extra = a.Extra,
                            Deadline = a.Deadline,
                            Station_Warranti = a.Station_Warranti,
                            Station = st.FullName,
                            KTV_Warranti = a.KTV_Warranti,
                            KTV = kt.FullName,
                            Successdate = a.Successdate,
                            Successnote = a.Successnote,
                            SuccessExtra = a.SuccessExtra,
                            Lat = a.Lat,
                            Long = a.Long,
                            KM = a.KM,
                            Price_Move = a.Price_Move,
                            Price_Accessary = a.Price_Accessary,
                            Price_Cate = a.Price_Cate,
                            Price = a.Price,
                            Service_Name = a.Service_Name,
                            Service_Price = a.Service_Price,
                            Amount = a.Amount,
                            Sign = a.Sign,

                            ProductCode = a.ProductCode,
                            Model = a.Model,
                            Serial = a.Serial,

                            CusName = c.Name,
                            //Activedate = p.Active_date,
                            //Limited = p.Limited,
                            //Enddate = p.End_date,

                            ProductName = a.ProductName,
                            BuyAdr = a.BuyAdr,
                            Buydate = a.Buydate,

                            Warranti_Accessaries = db.Warranti_Accessary.Where(w => w.IdWarranti == a.Id).ToList(),
                            Image = db.Warranti_Image.Where(w => w.IdWarranti == a.Id).ToList(),
                        };
            //hiển thị theo quyền
            if (User.IsInRole("Trạm - Trưởng trạm"))
            {
                model = model.Where(a => a.Station_Warranti == UserName || a.Createby == UserName);
            }
            if (User.IsInRole("Trạm - Nhân viên kỹ thuật"))
            {
                model = model.Where(a => a.KTV_Warranti == UserName || a.Createby == UserName);
            }
            if (User.IsInRole("Đại lý") || User.IsInRole("Nhân viên"))
            {
                model = model.Where(a => a.Createby == UserName);
            }
            //lọc theo thông tin 
            DateTime today = DateTime.Now.AddDays(3);
            var countstatus = new countstatus()
            {
                all = model.Count(),
                s0 = model.Where(a => a.Status == 0).Count(),
                s1 = model.Where(a => a.Status == 1).Count(),
                s2 = model.Where(a => a.Status == 2).Count(),
                s3 = model.Where(a => a.Status == 3).Count(),
                s4 = model.Where(a => a.Status == 4).Count(),
                s5 = model.Where(a => a.Status == 5).Count(),
                s6 = model.Where(a => a.Status == 6).Count(),
                s7 = model.Where(a => a.Status == 7).Count(),
                s8 = model.Where(a => a.Status == 8).Count(),
                s9 = model.Where(a => a.Status == 9).Count(),
                s11 = model.Where(a => a.Status == 11).Count(),
                outdate = model.Where(a => a.Status == 2 && a.Status == 5 && a.Deadline < today).Count(),
            };
            ViewBag.countstatus = countstatus;
            if (!string.IsNullOrEmpty(phone))
            {
                model = model.Where(a => a.Phone.Contains(phone));
                ViewBag.phone = phone;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.Contains(textsearch)
                || a.Createby.Contains(textsearch)
                || a.Note.Contains(textsearch)
                || a.Phone.Contains(textsearch)
                //|| a.PhoneExtra.Contains(textsearch)
                || a.Address.Contains(textsearch)
                //|| a.CusName.Contains(textsearch)
                || a.ProductCode.Contains(textsearch)
                || a.Model.Contains(textsearch)
                || a.Serial.Contains(textsearch)
                );
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(station))
            {
                model = model.Where(a => a.Station_Warranti.Contains(station)
                || a.KTV_Warranti.Contains(station));
                ViewBag.station = station;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Chanel.Contains(chanel));
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(status))
            {
                model = model.Where(a => a.Status.ToString().Contains(status));
                ViewBag.status = status;
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
            if (!string.IsNullOrEmpty(im_start_date))
            {
                DateTime s = DateTime.ParseExact(im_start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Successdate >= s);
                ViewBag.im_start_date = im_start_date;
            }
            if (!string.IsNullOrEmpty(im_end_date))
            {
                DateTime s = DateTime.ParseExact(im_end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Successdate <= s);
                ViewBag.im_end_date = im_end_date;
            }
            listexc = model as IEnumerable<WarrantiDetail>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Create()
        {
            return PartialView("~/Areas/Admin/Views/Warranti/_Create.cshtml", null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConfirm([Bind(Include = "")] Warranti warranti, string CusName)
        {
            if (ModelState.IsValid)
            {
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    SetAlert("Số điện thoại không đúng", "danger");
                //    return RedirectToAction("Index");
                //}
                var _customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (_customer == null)
                {
                    var customer = new Customer()
                    {
                        Name = CusName,
                        Phone = warranti.Phone,
                        Address = warranti.Address,
                        Ward = warranti.Ward,
                        District = warranti.District,
                        Province = warranti.Province,
                        Createdate = DateTime.Now,
                    };
                    db.Customers.Add(customer);
                }
                else
                {
                    _customer.Name = CusName;
                    _customer.Address = warranti.Address;
                    _customer.Ward = warranti.Ward;
                    _customer.District = warranti.District;
                    _customer.Province = warranti.Province;
                    db.Entry(_customer).State = EntityState.Modified;

                }
                warranti.Createby = User.Identity.Name;
                warranti.Createdate = DateTime.Now;
                warranti.Status = 0;
                warranti.Chanel = "CMS";
                //nếu được trạm tạo thì chuyển luôn cho trạm 
                if (User.IsInRole("Trạm - Trưởng trạm"))
                {
                    warranti.Status = 2;
                    warranti.Station_Warranti = User.Identity.Name;
                }
                db.Warrantis.Add(warranti);
                db.SaveChanges();

                warranti.Code = Utils.Control.CreateCode(warranti.Id);
                db.Entry(warranti).State = EntityState.Modified;

                var log = new Warranti_Log()
                {
                    Id_Warranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " Tạo mới phiếu bảo hành."
                };
                db.Warranti_Log.Add(log);
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult Views(long Id)
        {
            var model = db.Warrantis.Find(Id);
            var station = db.AspNetUsers.SingleOrDefault(a => a.UserName == model.Station_Warranti);
            var customer = db.Customers.SingleOrDefault(a => a.Phone == model.Phone);
            var product = db.Products.SingleOrDefault(a => a.Code == model.ProductCode);
            var list = from a in db.Warranti_Accessary
                       where a.IdWarranti == model.Id
                       join b in db.Accessary_Big on a.IdAccessary equals b.Id
                       select new PhuTungPrint()
                       {
                           Name = a.Name,
                           Code = b.Code
                       };
            PrintModel print = new PrintModel();
            print.Code = model.Code;
            if (station != null)
            {
                print.Station = station.FullName;
                print.StationAddress = station.Address + " " + station.Ward + " " + station.District + " " + station.Province;
                print.StationPhone = station.PhoneNumber;
            }

            if (customer != null)
            {
                print.CusName = customer.Name;
            }

            print.CusPhone = model.Phone;
            print.CusAddress = model.Address + " " + model.Ward + " " + model.District + " " + model.Province;
            if (product != null)
            {
                print.ProdName = product.Name;
            }
            print.ProdCode = model.ProductCode;
            print.Serial = model.Serial;
            print.Model = model.Model;
            if (model.Buydate != null)
            {
                print.Buydate = model.Buydate.Value.ToString("dd/MM/yyyy");
            }
            print.BuyAdr = model.BuyAdr;
            print.CusPhoneExtra = model.PhoneExtra;
            print.Note = model.Note;
            print.Cate = model.Cate;
            print.Warranti_Cate = model.Warranti_Cate;
            print.PhuTung_Recevice = "";
            print.Createdate = model.Createdate.Value.ToString("dd/MM/yyyy");
            print.Extra = model.Extra;
            print.Successnote = model.Successnote;
            print.Amount = model.Amount.ToString();
            print.PhuTung_Export = list.ToList();

            return PartialView("~/Areas/Admin/Views/Warranti/_Views.cshtml", print);
        }
        [HttpPost]
        public ActionResult Edit(long Id)
        {
            var model = db.Warrantis.Find(Id);
            return PartialView("~/Areas/Admin/Views/Warranti/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Warranti warranti, string CusName)
        {
            if (ModelState.IsValid)
            {
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    SetAlert("Số điện thoại không đúng", "danger");
                //    return RedirectToAction("Index");
                //}
                var _warranti = db.Warrantis.Find(warranti.Id);
                var log = new Warranti_Log()
                {
                    Id_Warranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " Chỉnh sửa thông tin phiếu."
                };
                db.Warranti_Log.Add(log);
                var customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (customer != null)
                {
                    customer.Name = CusName;
                    db.Entry(customer).State = EntityState.Modified;
                }

                _warranti.Phone = warranti.Phone;
                _warranti.PhoneExtra = warranti.PhoneExtra;
                _warranti.Province = warranti.Province;
                _warranti.District = warranti.District;
                _warranti.Ward = warranti.Ward;
                _warranti.Address = warranti.Address;

                _warranti.ProductCode = warranti.ProductCode;
                _warranti.Model = warranti.Model;
                _warranti.Serial = warranti.Serial;
                _warranti.ProductName = warranti.ProductName;

                _warranti.Note = warranti.Note;
                _warranti.Cate = warranti.Cate;
                _warranti.Warranti_Cate = warranti.Warranti_Cate;
                db.Entry(_warranti).State = EntityState.Modified;
                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }
        public ActionResult Detail(long Id)
        {
            var model = from a in db.Warrantis
                        join cus in db.Customers on a.Phone equals cus.Phone into tempc
                        from c in tempc.DefaultIfEmpty()
                        join b in db.Products on a.ProductCode equals b.SerialBrand into temp
                        from p in temp.DefaultIfEmpty()

                        join s in db.AspNetUsers on a.Station_Warranti equals s.UserName into temps
                        from st in temps.DefaultIfEmpty()
                        join k in db.AspNetUsers on a.KTV_Warranti equals k.UserName into tempk
                        from kt in tempk.DefaultIfEmpty()

                        select new WarrantiDetail()
                        {
                            Id = a.Id,
                            Status = a.Status,
                            Code = a.Code,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Chanel = a.Chanel,
                            Phone = a.Phone,
                            PhoneExtra = a.PhoneExtra,
                            Address = a.Address,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                            Cate = a.Cate,
                            Warranti_Cate = a.Warranti_Cate,
                            Note = a.Note,
                            Extra = a.Extra,
                            Deadline = a.Deadline,
                            Station_Warranti = a.Station_Warranti,
                            Station = st.FullName,
                            KTV_Warranti = a.KTV_Warranti,
                            KTV = kt.FullName,
                            Successdate = a.Successdate,
                            Successnote = a.Successnote,
                            SuccessExtra = a.SuccessExtra,
                            Lat = a.Lat,
                            Long = a.Long,
                            KM = a.KM,
                            Price_Move = a.Price_Move,
                            Price_Accessary = a.Price_Accessary,
                            Price_Cate = a.Price_Cate,
                            Price = a.Price,
                            Service_Name = a.Service_Name,
                            Service_Price = a.Service_Price,
                            Amount = a.Amount,
                            Sign = a.Sign,
                            ProductCode = a.ProductCode,
                            Model = a.Model,
                            Serial = a.Serial,

                            CusName = c.Name,
                            Limited = p.Limited,
                            Enddate = p.End_date,
                            Activedate = p.Active_date,

                            ProductName = a.ProductName,
                            Buydate = a.Buydate,
                            BuyAdr = a.BuyAdr,

                            Warranti_Accessaries = db.Warranti_Accessary.Where(a => a.IdWarranti == Id).ToList(),
                            Image = db.Warranti_Image.Where(w => w.IdWarranti == a.Id).ToList(),
                        };
            WarrantiDetail warranti = model.FirstOrDefault(a => a.Id == Id);
            return View(warranti);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailConfirm([Bind(Include = "")] WarrantiDetail warranti, IEnumerable<HttpPostedFileBase> Image)
        {
            try
            {
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    SetAlert("Số điện thoại không đúng", "danger");
                //    return RedirectToAction("Index");
                //}
                var _waranti = db.Warrantis.Find(warranti.Id);
                //luu anh vao phieu neu co

                var listimage = new List<Warranti_Image>();
                foreach (var item in Image)
                {
                    if (item != null)
                    {
                        string strrand = Guid.NewGuid().ToString();
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/Data/ImageWarr/"), strrand + "_" + fileName);
                        item.SaveAs(path);
                        ResizeSettings resizeSetting = new ResizeSettings
                        {
                            MaxWidth = 800,
                            MaxHeight = 800,
                        };
                        ImageBuilder.Current.Build(path, path, resizeSetting);
                        string link = "/Data/ImageWarr/" + strrand + "_" + fileName;

                        var image = new Warranti_Image()
                        {
                            Image = link,
                            IdWarranti = _waranti.Id,
                        };
                        listimage.Add(image);
                    }

                }
                db.Warranti_Image.AddRange(listimage);

                //lưu log chi tiet
                if (_waranti.Status != warranti.Status)
                {
                    var log = new Warranti_Log()
                    {
                        Id_Warranti = warranti.Id,
                        Createdate = DateTime.Now,
                        Description = User.Identity.Name + " Cập nhật trạng thái phiếu. " + Common.getStatusstring(warranti.Status)
                    };
                    db.Warranti_Log.Add(log);
                }
                else
                {
                    var log = new Warranti_Log()
                    {
                        Id_Warranti = warranti.Id,
                        Createdate = DateTime.Now,
                        Description = User.Identity.Name + " Cập nhật chi tiết phiếu."
                    };
                    db.Warranti_Log.Add(log);
                }
                //Khi trạng thái phiếu là Đang xử lý, KTV ấn nút hoàn thành, 
                if (_waranti.Status == 5 && _waranti.KTV_Warranti != null && _waranti.KTV_Warranti.Equals(User.Identity.Name))
                {
                    //chuyển trạng thái từ đang xử lý thành chờ phản hồi và fill ngày hoàn thành
                    _waranti.Status = 8;
                    _waranti.Successdate = DateTime.Now;
                }
                else
                {
                    //chuyển trạng thái
                    int changeStatus = warranti.Status;
                    if (changeStatus == _waranti.Status)
                    {
                        //tự chuyển trạng thái
                        _waranti.Status = SetStatus(_waranti.Status, warranti, (warranti.Warranti_Accessaries != null));
                    }
                    else
                    {
                        //chuyển khi có thao tác
                        _waranti.Status = changeStatus;
                    }
                }
                
                //gửi notifi cho trạm
                if (warranti.Station != _waranti.Station_Warranti && string.IsNullOrEmpty(_waranti.Station_Warranti))
                {
                    var sent = new SentNotify();
                    sent.Sent(warranti.Station_Warranti, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.Code));
                }
                //gửi notifi cho ktv
                if (warranti.KTV_Warranti != _waranti.KTV_Warranti && string.IsNullOrEmpty(_waranti.KTV_Warranti))
                {
                    var sent = new SentNotify();
                    sent.Sent(warranti.KTV_Warranti, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.Code));
                }

                //lưu thông tin khách hàng vào phiếu bảo hành
                var customer = db.Customers.FirstOrDefault(a => a.Phone == warranti.Phone);
                if (customer != null)
                {
                    customer.Name = warranti.CusName;
                    db.Entry(customer).State = EntityState.Modified;
                }
                _waranti.Phone = warranti.Phone;
                _waranti.PhoneExtra = warranti.PhoneExtra;
                _waranti.Province = warranti.Province;
                _waranti.District = warranti.District;
                _waranti.Ward = warranti.Ward;
                _waranti.Address = warranti.Address;

                //lưu thông tin sản phẩm vào phiếu

                _waranti.ProductCode = warranti.ProductCode;
                _waranti.Model = warranti.Model;
                _waranti.Serial = warranti.Serial;
                _waranti.Buydate = warranti.Buydate;
                _waranti.BuyAdr = warranti.BuyAdr;
                _waranti.ProductName = warranti.ProductName;

                //lưu thông tin chi tiết phiếu

                _waranti.Deadline = warranti.Deadline;
                //if (_waranti.Status == 11)
                //{
                //    _waranti.Successdate = DateTime.Now;
                //}
                //else
                //{
                //    _waranti.Successdate = warranti.Successdate;
                //}
                _waranti.Cate = warranti.Cate;
                _waranti.Warranti_Cate = warranti.Warranti_Cate;
                _waranti.Note = warranti.Note;
                _waranti.Extra = warranti.Extra;
                _waranti.Station_Warranti = warranti.Station_Warranti;
                _waranti.KTV_Warranti = warranti.KTV_Warranti;
                _waranti.Date_KTV = warranti.Date_KTV;
                _waranti.Successnote = warranti.Successnote;
                _waranti.SuccessExtra = warranti.SuccessExtra;
                //lưu danh sách linh kiện dùng
                var items_acc = db.Warranti_Accessary.Where(a => a.IdWarranti == _waranti.Id);
                int lkcu = items_acc.Count();
                int lkmoi = (warranti.Warranti_Accessaries != null) ? warranti.Warranti_Accessaries.Count() : 0;

                if (lkmoi != lkcu || lkmoi != 0)
                {
                    //duyet lkmoi bo qua linh kien cu
                    //đã có linh kiện cũ thì bắt đầu từ lkmoi, chưa có thì bắt đầu từ 0

                    int lkbatdau = (lkmoi > lkcu) ? (lkmoi - lkcu) : 0;
                    for (int i = lkcu; i < lkmoi; i++)
                    {
                        if (warranti.Warranti_Accessaries[i].Name == null)
                        {
                            continue;
                        }
                        var _lkthem = warranti.Warranti_Accessaries[i];
                        //trừ đi linh kiện
                        var check_in_big = db.Accessary_Big.Find(_lkthem.IdAccessary);
                        //check o tram
                        var check_in_key = db.Accessary_Key.FirstOrDefault(a => a.Code == check_in_big.Code && a.Id_Key == warranti.Station_Warranti);
                        
                        if (check_in_key!=null)
                        {
                            bool conlinhkien = check_in_key.CountImport - check_in_key.CountExport > 0;
                            if (conlinhkien)
                            {
                                //tru linh kien o tram
                                check_in_key.CountExport = check_in_key.CountExport + 1;
                                db.Entry(check_in_key).State = EntityState.Modified;
                                //luu linh kien vao phieu
                                var acc = new Warranti_Accessary()
                                {
                                    IdWarranti = warranti.Id,
                                    Name = _lkthem.Name,
                                    Price = _lkthem.Price,
                                    Quantity = 1,
                                    Amount = _lkthem.Price,
                                    IdAccessary_key = check_in_key.Id
                                };
                                db.Warranti_Accessary.Add(acc);
                                //add log xuat linh kien
                                var log_akey = new Acessary_Log_Key()
                                {
                                    Accessary = acc.Name,
                                    Code = warranti.Code,
                                    Count = 1,
                                    Createdate = DateTime.Now,
                                    Id_Akey = check_in_key.Id,
                                    Type = 2//xuat
                                };
                                db.Acessary_Log_Key.Add(log_akey);
                            }
                            else
                            {
                                //thong bao
                                SetAlert("Linh kiện không có sẵn.", "danger");
                                return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });
                            }
                        }
                        else
                        {
                            //thong bao
                            SetAlert("Không có linh kiện.", "danger");
                            return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });

                        }                                           

                    }

                }
                _waranti.KM = warranti.KM;
                _waranti.Price_Move = warranti.Price_Move;
                _waranti.Price_Cate = warranti.Price_Cate;
                _waranti.Service_Price = warranti.Service_Price;
                _waranti.Service_Name = warranti.Service_Name;
                _waranti.Price = warranti.Price;
                _waranti.Amount = warranti.Amount;

                db.Entry(_waranti).State = EntityState.Modified;

                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Detail", "Warranti", new { Id = warranti.Id });

        }
        public ActionResult BackLK(long Id, long LK)
        {
            
            try
            {
                var warranti = db.Warrantis.Find(Id);
                
                var linhkientrongphieu = db.Warranti_Accessary.FirstOrDefault(a => a.IdWarranti == Id && a.Id == LK);
                var log_key = db.Acessary_Log_Key.FirstOrDefault(a => a.Code == warranti.Code && a.Id_Akey == linhkientrongphieu.IdAccessary_key);
                //long id_log = long.Parse(log_key.Id_Akey);
                var tram = db.Accessary_Key.FirstOrDefault(a => a.Id == linhkientrongphieu.IdAccessary_key);
                //update lại số lk ở trạm
                tram.CountExport = tram.CountExport - 1;
                db.Entry(tram).State = EntityState.Modified;

                //xoá linh kiện trong phiếu
                db.Warranti_Accessary.Remove(linhkientrongphieu);
                //xoá log xuất lk
                db.Acessary_Log_Key.Remove(log_key);
                //nếu tính phí thì phải trừ tiền đi
                if (warranti.Price_Accessary > 0)
                {
                    warranti.Price_Accessary = warranti.Price_Accessary - linhkientrongphieu.Price;
                    warranti.Amount = warranti.Amount - linhkientrongphieu.Price;
                    db.Entry(warranti).State = EntityState.Modified;
                }
                db.SaveChanges();
                SetAlert("Đã trả linh kiện thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert("Trả linh kiện không thành công. Hãy kiểm tra lại.", "danger");
            }

            return RedirectToAction("Detail", "Warranti", new { Id = Id });
        }
        public ActionResult DeleteImage(long Id)
        {
            var image = db.Warranti_Image.Find(Id);
            db.Warranti_Image.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Detail", "Warranti", new { Id = image.IdWarranti });
        }
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.Warrantis.Find(Id);
                db.Warrantis.Remove(model);
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
        public ActionResult ViewLog(long Id)
        {
            var log = db.Warranti_Log.Where(a => a.Id_Warranti == Id);
            ViewBag.Code = db.Warrantis.Find(Id).Code;
            return PartialView("~/Areas/Admin/Views/Warranti/_Viewlog.cshtml", log.ToList());
        }

        [HttpPost]
        public JsonResult GetAccessary(string text, string station)
        {
            var cate = (from a in db.Accessary_Big
                        where a.Code.Contains(text)
                        where a.Name.Contains(text)
                        select new { a.Name, a.Id, a.Code });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckvalidAccess(string text, string station)
        {
            var check_in_key = db.Accessary_Key.FirstOrDefault(a => a.Code == text && a.Id_Key == station);            
            if(check_in_key!=null)
            {
                bool conlinhkien = check_in_key.CountImport - check_in_key.CountExport > 0;
                if (conlinhkien)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }                
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCate(string text)
        {
            var cate = (from a in db.Type_Err
                        where a.Description.Contains(text) || a.Name.Contains(text)
                        select new { a.Description, a.Name});
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetStation(string text)
        {
            if (User.IsInRole("Admin - Quản trị toàn hệ thống"))
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == "Key"
                            where a.UserName.Contains(text) || a.FullName.Contains(text)
                            select new { a.UserName, a.FullName });
                return Json(cate.Take(10), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var cate = (from a in db.AspNetUsers
                            from b in a.AspNetRoles
                            where b.Id == "Key"
                            where a.Createby == User.Identity.Name
                            where a.UserName.Contains(text) || a.FullName.Contains(text)
                            select new { a.UserName, a.FullName });
                return Json(cate.Take(10), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult GetKTV(string text, string station)
        {
            var cate = (from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where b.Id == "KTV"
                        where a.Createby == station
                        where a.UserName.Contains(text) || a.FullName.Contains(text)
                        select new { a.UserName, a.FullName });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetKM(string text)
        {
            var cate = (from a in db.Move_Price
                        where a.Name.Contains(text)
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetPrice(string text, string modell)
        {
            var getModel = db.P_Model.Where(a => a.Model == modell).FirstOrDefault();
            var capacity = getModel.Capacity;
            var cate = (from a in db.Repair_Price
                        where a.Name.Contains(text) && a.Capacity == capacity
                        select new { a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetService(string text, string modell)
        {
            var getModel = db.P_Model.Where(a => a.Model == modell).FirstOrDefault();
            var capacity = getModel.Capacity;
            var service = (from a in db.Service_Price
                        where a.Name.Contains(text) && a.Capacity == capacity
                        select new { a.Name });
            return Json(service, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceMove(string text)
        {
            var product = db.Move_Price.Where(a => a.Name == text).SingleOrDefault();
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceAccess(long Id)
        {
            var product = db.Accessary_Big.Find(Id);
            return Json(product.KeyPrice, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceCate(string text, string model)
        {
            var getModel = db.P_Model.Where(a=>a.Model == model).FirstOrDefault();
            var capacity = getModel.Capacity;
            var product = db.Repair_Price.Where(a => a.Name == text && a.Capacity == capacity).FirstOrDefault();
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetPriceService(string text, string model)
        {
            var getModel = db.P_Model.Where(a => a.Model == model).FirstOrDefault();
            var capacity = getModel.Capacity;
            var product = db.Service_Price.Where(a => a.Name == text && a.Capacity == capacity).FirstOrDefault();
            return Json(product.Price, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetProduct(string text)
        {
            var product = db.Products.Where(a => a.SerialBrand == text).FirstOrDefault();
            if (product != null)
            {
                var result = new ProductResult()
                {
                    Name = product.Name,
                    Limited = product.Limited.ToString(),
                    Activedate = product.Active_date.Value.ToString("yyyy/MM/dd"),
                    Enddate = product.End_date.Value.ToString("yyyy/MM/dd"),
                    Model = product.Model,
                    Code = product.Code,
                    Serial = product.Serial,
                    Buydate = product.End_date.Value.ToString("yyyy/MM/dd"),
                    BuyAdr = product.AgentC1
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult LoadCustomer(string text)
        {
            var customer = db.Customers.Where(a => a.Phone == text).SingleOrDefault();
            var result = new CustomerResult()
            {
                Name = customer.Name,
                Province = customer.Province,
                District = customer.District,
                Ward = customer.Ward,
                Address = customer.Address
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GetCustomer(string phone)
        {
            var customer = db.Customers.Where(a => a.Phone == phone).SingleOrDefault();
            if (customer == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var product = db.Products.Where(a => a.Active_phone == customer.Phone);
                var result = new CustomerResult()
                {
                    Name = customer.Name,
                    Province = customer.Province,
                    District = customer.District,
                    Ward = customer.Ward,
                    Address = customer.Address,
                    ListProduct = product.ToList()
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }
        [HttpPost]
        public JsonResult GetCode(string text)
        {
            var cate = (from a in db.Products
                        where a.Code.Contains(text)
                        select new { a.Code, a.Serial, a.Model, a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSerial(string text)
        {
            var cate = (from a in db.Products
                        where a.Serial.Contains(text)
                        select new { a.Code, a.Serial, a.Model, a.Name });
            return Json(cate, JsonRequestBehavior.AllowGet);
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
        public int SetStatus(int status, Warranti warranti, bool isAccessary)
        {
            if (warranti.Status == 1)
            {
                warranti.Status = 10;
            }
            int result = status;
            //chuyen tram
            if (warranti.Station_Warranti != null && warranti.Status < 2)
            {
                result = 2;
            }
            //dang xu ly
            if (warranti.KTV_Warranti != null && warranti.Status < 5)
            {
                result = 5;
            }
            //cho phan hoi
            if ((isAccessary) && warranti.Status < 8)
            {
                result = 8;
            }
            //chua co linh kien ma da chon hoan thanh
            if ((!isAccessary) && (warranti.Status == 10 || warranti.Successdate != null))
            {
                result = 7;
            }

            if (User.IsInRole("Trạm - Trưởng trạm") || User.IsInRole("Admin - Quản trị toàn hệ thống"))
            {
                if (warranti.Status > result)
                {
                    result = warranti.Status;
                    if (result == 10)
                    {
                        result = 1;//tra về trạng thái đúng
                    }
                }
            }
            return result;
        }
        public class countstatus
        {
            public int all { get; set; }
            public int s0 { get; set; }
            public int s1 { get; set; }
            public int s2 { get; set; }
            public int s3 { get; set; }
            public int s4 { get; set; }
            public int s5 { get; set; }
            public int s6 { get; set; }
            public int s7 { get; set; }
            public int s8 { get; set; }
            public int s9 { get; set; }
            public int s11 { get; set; }
            public int outdate { get; set; }
        }

        public void Phieubaohanh()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");

            //Sheet.Cells["A1"].Value = "";
            //Sheet.Cells["A2"].Value = "";
            //Sheet.Cells["A3"].Value = "";
            //Sheet.Cells["A4"].Value = "";
            //Sheet.Cells["E5"].Value = "";
            //Sheet.Cells["A6"].Value = "";

            //Sheet.Cells["A1"].Style.Font.Bold = true;
            //Sheet.Cells["A2"].Style.Font.Bold = true;
            //Sheet.Cells["A3"].Style.Font.Bold = true;
            //Sheet.Cells["A4"].Style.Font.Bold = true;
            //Sheet.Cells["A5"].Style.Font.Bold = true;

            //Sheet.Cells["A5"].Style.Font.Italic = true;
            //Sheet.Cells["A6"].Style.Font.Italic = true;
            //Sheet.Cells["A5"].Style.Font.Size = 8;
            //Sheet.Cells["A6"].Style.Font.Size = 8;

            //Sheet.Cells["A9:S9"].Style.Fill.BackgroundColor.SetColor(Color.Gray);

            Sheet.Cells["A9:S9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A9:S9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A9:S9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A9:S9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A10:S10"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A10:S10"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A10:S10"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            Sheet.Cells["A10:S10"].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            //Sheet.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //Sheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Sheet.Cells["A1:S1"].Merge = true;
            //Sheet.Cells["A2:S2"].Merge = true;
            //Sheet.Cells["A3:S3"].Merge = true;

            //Sheet.Cells["A4:K4"].Merge = true;
            //Sheet.Cells["E5:J5"].Merge = true;
            //Sheet.Cells["A6:K6"].Merge = true;


            Sheet.Cells["A9"].Value = "";
            Sheet.Cells["B9"].Value = "Trạm BH";
            Sheet.Cells["C9"].Value = "Ngày tạo";
            Sheet.Cells["D9"].Value = "Ngày hoàn thành";
            Sheet.Cells["E9"].Value = "Sản phẩm";
            Sheet.Cells["F9"].Value = "Model";
            Sheet.Cells["G9"].Value = "Serial";
            Sheet.Cells["H9"].Value = "Ngày mua";
            Sheet.Cells["I9"].Value = "Mô tả";
            Sheet.Cells["J9"].Value = "Phân loại phiếu";
            Sheet.Cells["K9"].Value = "Tên linh kiện";
            Sheet.Cells["L9"].Value = "Mã linh kiện";
            Sheet.Cells["M9"].Value = "Số lượng";
            Sheet.Cells["N9"].Value = "Mã phiếu bảo hành";
            Sheet.Cells["O9"].Value = "Họ tên";
            Sheet.Cells["P9"].Value = "Số điện thoại";
            Sheet.Cells["Q9"].Value = "Địa chỉ";
            Sheet.Cells["R9"].Value = "Tổng tiền";
            Sheet.Cells["S9"].Value = "Phân loại phiếu = bảo hành thì đánh dấu X";

            Sheet.Cells["A10"].Value = "STT";
            Sheet.Cells["B10"].Value = "Trạm BH";
            Sheet.Cells["C10"].Value = "NGÀY NHẬN";
            Sheet.Cells["D10"].Value = "NGÀY TRẢ";
            Sheet.Cells["E10"].Value = "TÊN SẢN PHẨM";
            Sheet.Cells["F10"].Value = "MODEL";
            Sheet.Cells["G10"].Value = "SERIAL";
            Sheet.Cells["H10"].Value = "NGÀY MUA";
            Sheet.Cells["I10"].Value = "HIỆN TƯỢNG HƯ HỎNG";
            Sheet.Cells["J10"].Value = "HÌNH THỨC SỬA CHỮA";
            Sheet.Cells["K10"].Value = "LINH KIỆN THAY THẾ";
            Sheet.Cells["L10"].Value = "MÃ LINH KIỆN";
            Sheet.Cells["M10"].Value = "SỐ LƯỢNG";
            Sheet.Cells["N10"].Value = "MÃ BIÊN NHẬN SỬA CHỮA";
            Sheet.Cells["O10"].Value = "TÊN KHÁCH HÀNG";
            Sheet.Cells["P10"].Value = "SỐ ĐIỆN THOẠI";
            Sheet.Cells["Q10"].Value = "ĐỊA CHỈ";
            Sheet.Cells["R10"].Value = "Thành tiền";
            Sheet.Cells["S10"].Value = "BẢO HÀNH";


            Sheet.Cells["A10:S10"].Style.Font.Bold = true;

            int index = 1;
            int row = 11;
            foreach (var item in listexc)
            {
                string war = item.Warranti_Cate;
                if (war == "Bảo hành")
                {
                    war = "X";
                }
                string acessary = "";
                string amount = "";
                string code = "";
                string count = "";
                foreach (var jtem in item.Warranti_Accessaries)
                {
                    count += "1" + "\n";
                    acessary += jtem.Name + "\n";
                    amount += jtem.Amount.ToString() + "\n";
                    try
                    {
                        var ass = db.Accessary_Key.FirstOrDefault(a => a.Name == jtem.Name);
                        code += ass.Code + "\n";
                    }
                    catch (Exception ex)
                    {
                        code += "-" + "\n";
                    }

                }
                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Station_Warranti;
                Sheet.Cells[string.Format("C{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("D{0}", row)].Value = (item.Successdate != null) ? item.Successdate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductName;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Model;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Serial;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Buydate != null) ? item.Buydate.Value.ToString("dd/MM/yyyy") : "";
                Sheet.Cells[string.Format("I{0}", row)].Value = item.Note;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Successnote;
                Sheet.Cells[string.Format("K{0}", row)].Value = acessary;
                Sheet.Cells[string.Format("L{0}", row)].Value = code;
                Sheet.Cells[string.Format("M{0}", row)].Value = count;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Code;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.CusName;
                Sheet.Cells[string.Format("P{0}", row)].Value = item.Phone;
                Sheet.Cells[string.Format("Q{0}", row)].Value = item.Address + " " + item.Ward + " " + item.District + " " + item.Province;
                Sheet.Cells[string.Format("R{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("S{0}", row)].Value = war;


                Sheet.Cells[string.Format("K{0}", row)].Style.WrapText = true;
                Sheet.Cells[string.Format("L{0}", row)].Style.WrapText = true;
                Sheet.Cells[string.Format("M{0}", row)].Style.WrapText = true;

                //Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                //Sheet.Cells[string.Format("B{0}", row)].Value = item.Code;
                //Sheet.Cells[string.Format("C{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";
                //Sheet.Cells[string.Format("D{0}", row)].Value = item.Chanel;
                //Sheet.Cells[string.Format("E{0}", row)].Value = item.Createby;
                //Sheet.Cells[string.Format("F{0}", row)].Value = item.Status;
                //Sheet.Cells[string.Format("G{0}", row)].Value = item.Warranti_Cate;
                //Sheet.Cells[string.Format("H{0}", row)].Value = item.Cate;
                //Sheet.Cells[string.Format("I{0}", row)].Value = item.Note;
                //Sheet.Cells[string.Format("J{0}", row)].Value = item.Extra;
                //Sheet.Cells[string.Format("K{0}", row)].Value = item.Phone;
                //Sheet.Cells[string.Format("L{0}", row)].Value = item.PhoneExtra;
                //Sheet.Cells[string.Format("M{0}", row)].Value = item.CusName;
                //Sheet.Cells[string.Format("N{0}", row)].Value = item.Address;
                //Sheet.Cells[string.Format("O{0}", row)].Value = item.ProductName;
                //Sheet.Cells[string.Format("P{0}", row)].Value = item.ProductCode;
                //Sheet.Cells[string.Format("Q{0}", row)].Value = item.Serial;
                //Sheet.Cells[string.Format("R{0}", row)].Value = item.Model;
                //Sheet.Cells[string.Format("S{0}", row)].Value = item.Station_Warranti;
                //Sheet.Cells[string.Format("T{0}", row)].Value = item.KTV_Warranti;
                //Sheet.Cells[string.Format("U{0}", row)].Value = (item.Deadline != null) ? item.Deadline.Value.ToString("dd/MM/yyyy") : "";
                //Sheet.Cells[string.Format("V{0}", row)].Value = (item.Successdate != null) ? item.Successdate.Value.ToString("dd/MM/yyyy") : "";
                //Sheet.Cells[string.Format("W{0}", row)].Value = item.Successnote;
                //Sheet.Cells[string.Format("X{0}", row)].Value = acessary;
                //Sheet.Cells[string.Format("Y{0}", row)].Value = code;
                //Sheet.Cells[string.Format("Z{0}", row)].Value = amount;
                //Sheet.Cells[string.Format("AA{0}", row)].Value = item.Price_Accessary;
                //Sheet.Cells[string.Format("AB{0}", row)].Value = item.KM;
                //Sheet.Cells[string.Format("AC{0}", row)].Value = item.Price_Move;
                //Sheet.Cells[string.Format("AD{0}", row)].Value = item.Price;
                //Sheet.Cells[string.Format("AE{0}", row)].Value = item.Amount;
                //Sheet.Cells[string.Format("AF{0}", row)].Value = item.SuccessExtra;

                //Sheet.Cells[string.Format("X{0}", row)].Style.WrapText = true;
                //Sheet.Cells[string.Format("Y{0}", row)].Style.WrapText = true;
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