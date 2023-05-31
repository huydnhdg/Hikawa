using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using WebApplication.APIFORAPP.Model;
using WebApplication.APIFORAPP.ModelRequest;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;
using WebApplication.Models.Client;
using WebApplication.Utils;

namespace WebApplication.APIFORAPP
{
    [RoutePrefix("api/action")]
    public class DefaultController : ApiController
    {
        HikawaEntities db = new HikawaEntities();
        Logger logger = LogManager.GetCurrentClassLogger();

        [Route("sendcode")]
        [HttpGet]
        public HttpResponseMessage Sendcode(String Phone)
        {
            //string MT = "Ma xac nhan cua ban la: {0}";
            var check = Utils.Control.getMobileOperator(Phone);
            if (!check.Equals("UNKNOWN"))
            {
                //tao ma xac nhan
                string PIN = Utils.Control.CreatePIN();
                //gui cho sdt
                //string status = "0";
                string status = SendBrandNameVMG.SendSMS(Phone, PIN);
                //luu vao db
                var sendcode = new SendCode()
                {
                    Phone = Phone,
                    Code = PIN,
                    Createdate = DateTime.Now,
                    SendStatus = int.Parse(status)
                };
                db.SendCodes.Add(sendcode);
                db.SaveChanges();
            }
            var result = new ResultString();

            result.Status = 1;
            result.Message = "OK";
            result.Data = new List<string>();

            return ResponseMessage(result);
        }
        [Route("getiframe")]
        [HttpGet]
        public HttpResponseMessage GetIframe(string Model)
        {
            var b_model = db.B_Model.FirstOrDefault(a => a.Model == Model);
            if (b_model != null)
            {
                var result = new ResultString();

                result.Status = 1;
                result.Message = b_model.Iframe;
                result.Data = new List<string>();

                return ResponseMessage(result);
            }
            else
            {
                var result = new ResultString();

                result.Status = -1;
                result.Message = "notvalid";
                result.Data = new List<string>();

                return ResponseMessage(result);
            }
        }
        [Route("checkUser")]
        [HttpGet]
        public HttpResponseMessage CheckUser(String Phone)
        {

            var user = db.AspNetUsers.SingleOrDefault(a => a.UserName == Phone);
            if (user != null)
            {
                var result = new ResultString();

                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<string>();

                return ResponseMessage(result);
            }
            else
            {
                var result = new ResultString();

                result.Status = 0;
                result.Message = "OK";
                result.Data = new List<string>();

                return ResponseMessage(result);
            }

        }
        [Route("checkotp")]
        [HttpGet]
        public HttpResponseMessage CheckOTP(String OTP, String Phone)
        {
            var sendcode = db.SendCodes.OrderByDescending(a => a.Createdate).Where(a => a.Phone == Phone).FirstOrDefault();
            var end = DateTime.Now;

            if (sendcode != null)
            {
                
                if(sendcode.Code == OTP && sendcode.CheckStatus == false && sendcode.Createdate.Value.AddSeconds(30) >= end)
                {
                    sendcode.CheckStatus = true;
                    db.Entry(sendcode).State = EntityState.Modified;
                    db.SaveChanges();

                    var result = new ResultString();

                    result.Status = 1;
                    result.Message = sendcode.Phone;
                    result.Data = new List<string>();

                    return ResponseMessage(result);
                }
                else
                {
                    var result = new ResultString();

                    result.Status = 0;
                    result.Message = "OK";
                    result.Data = new List<string>();

                    return ResponseMessage(result);
                }
            }
            else
            {
                var result = new ResultString();

                result.Status = 0;
                result.Message = "OK";
                result.Data = new List<string>();

                return ResponseMessage(result);
            }
        }

        //post
        [Route("updateinfo")]
        [HttpPost]
        public HttpResponseMessage UpdateInfo(ActiveReq active)
        {
            var result = new ProductActiveRes();
            if (string.IsNullOrEmpty(active.Province) || string.IsNullOrEmpty(active.District) || string.IsNullOrEmpty(active.Ward) || string.IsNullOrEmpty(active.Address))
            {
                result.Status = 1;
                result.Message = "Nhập thông tin địa chỉ";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            try
            {
                var customer = db.Customers.SingleOrDefault(a => a.Phone == active.Phone);
                if (customer == null)
                {
                    customer = new Customer();
                    customer.Phone = active.Phone;
                    customer.Email = active.Email;
                    customer.Name = active.CusName;
                    customer.Address = active.Address;
                    customer.Ward = active.Ward;
                    customer.District = active.District;
                    customer.Province = active.Province;
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                else
                {
                    customer.Phone = active.Phone;
                    customer.Email = active.Email;
                    customer.Name = active.CusName;
                    customer.Address = active.Address;
                    customer.Ward = active.Ward;
                    customer.District = active.District;
                    customer.Province = active.Province;
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                }
                result.Status = 1;
                result.Message = "Cập nhật thông tin thành công";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                result.Status = 1;
                result.Message = "Lưu không thành công";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
        }
        [Route("active")]
        [HttpPost]
        public HttpResponseMessage Active(ActiveReq active)
        {
            var result = new ProductActiveRes();
            //check SerialBrand
            if (string.IsNullOrEmpty(active.Code) || string.IsNullOrEmpty(active.SerialBrand))
            {
                result.Status = 1;
                result.Message = "Nhập Serial máy/ Mã cào sản phẩm";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            if (string.IsNullOrEmpty(active.Phone))
            {
                result.Status = 1;
                result.Message = "Nhập số điện thoại bảo hành";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            if (Utils.Control.getMobileOperator(active.Phone) == "UNKNOWN")
            {
                result.Status = 1;
                result.Message = "Số điện thoại không đúng";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            //if (string.IsNullOrEmpty(active.Province) || string.IsNullOrEmpty(active.District) || string.IsNullOrEmpty(active.Ward) || string.IsNullOrEmpty(active.Address))
            //{
            //    result.Status = 1;
            //    result.Message = "Nhập thông tin địa chỉ";
            //    result.Data = new List<ProductResult>();
            //    return ResponseMessage(result);
            //}
            try
            {
                //check SerialBrand va check code bangr khacs
                var product = db.Products.SingleOrDefault(a => a.SerialBrand == active.SerialBrand);
                var productstamps = db.ProductStamps.FirstOrDefault(a => a.Code == active.Code);
                var _agency = db.AspNetUsers.SingleOrDefault(a => a.UserName == active.Active_by);
                if (product == null)
                {
                    result.Status = 1;
                    result.Message = "Serial máy sản phẩm không tồn tại trong hệ thống";
                    result.Data = new List<ProductResult>();
                }
                else if (product.Status == 1)
                {
                    result.Status = 1;
                    result.Message = "Serial máy sản phẩm đã được kích hoạt trước đó";
                    result.Data = new List<ProductResult>();
                }
                else if(productstamps == null)
                {
                    result.Status = 1;
                    result.Message = "Tem cào sản phẩm không tồn tại trong hệ thống";
                    result.Data = new List<ProductResult>();
                }
                else if(productstamps.Status == 1)
                {
                    result.Status = 1;
                    result.Message = "Tem cào sản phẩm đã được kích hoạt trước đó";
                    result.Data = new List<ProductResult>();
                }
                else if (_agency != null && _agency.PhoneNumber == active.Phone)
                {
                    result.Status = 1;
                    result.Message = "Số điện thoại không thỏa điều kiện kích hoạt sản phẩm";
                    result.Data = new List<ProductResult>();
                }
                else
                {
                    //nếu ngày chờ kích hoạt quá ngày wait_time thì sử lý khác
                    //ngày kích hoạt là ngày chờ + số ngày wait_day
                    int wait_time = product.Wait_day;
                    var date = DateTime.Now.AddDays(-wait_time);
                    if (product.Status == 3 && product.Wait_date < date)
                    {
                        product.Customer_date = DateTime.Now;
                        product.Active_date = product.Wait_date.Value.AddDays(wait_time);
                        product.End_date = product.Active_date.Value.AddMonths(product.Limited);
                    }
                    else
                    {
                        product.Active_date = DateTime.Now;
                        product.End_date = DateTime.Now.AddMonths(product.Limited);
                    }
                    string msg = "";
                    //tao hoac update thong tin khach hang
                    var old_customer = db.Customers.SingleOrDefault(a => a.Phone == active.Phone);
                    if (old_customer == null)
                    {
                        //taoj moi thong tin khacsh hang
                        old_customer = new Customer()
                        {
                            Createdate = DateTime.Now,
                            Phone = active.Phone,
                            Chanel = active.Chanel,
                            Name = active.CusName,
                            Province = active.Province,
                            District = active.District,
                            Ward = active.Ward,
                            Address = active.Address
                        };
                        db.Customers.Add(old_customer);
                        db.SaveChanges();
                    }
                    else
                    {
                        //update thong tin khach hang
                        if (!string.IsNullOrEmpty(old_customer.Province))
                        {
                            old_customer.Email = active.Email;
                            old_customer.Name = active.CusName;
                            old_customer.Province = active.Province;
                            old_customer.District = active.District;
                            old_customer.Ward = active.Ward;
                            old_customer.Address = active.Address;

                            db.Entry(old_customer).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                    }
                    //dai ly kich hoat
                    product.Active_by = active.Active_by;
                    //khach  hang tu kich hoat
                    product.Active_phone = active.Phone;
                    product.Status = 1;
                    //check va update Ma cao va doi trang thai ma cao
                    productstamps.Status = 1;
                    product.Code = productstamps.Code;
                    product.Serial = productstamps.Serial;
                    product.Active_chanel = active.Chanel;
                    if (!string.IsNullOrEmpty(active.Buydate))
                    {
                        product.Buydate = DateTime.ParseExact(active.Buydate, "dd/MM/yyyy", null);
                    }

                    db.Entry(product).State = EntityState.Modified;
                    db.Entry(productstamps).State = EntityState.Modified;
                    //thưởng kích hoạt cho đại lý hoặc nhân viên
                    //thưởng kích hoạt cho đại lý tài khoản có phải đại lý không
                    if (ConfigControl.isBonusActive() == true)
                    {
                        //log thưởng cho đại lý theo sản phẩm
                        var agent_active = new Agent_Log_Active()
                        {
                            Createdate = DateTime.Now,
                            Model = product.Model,
                            Product = product.Code,
                            UserName = product.Active_by,
                            Amount = Getpricemodel(product.Model)
                        };
                        db.Agent_Log_Active.Add(agent_active);
                        //check role

                        if (_agency != null)
                        {
                            var role = _agency.AspNetRoles.FirstOrDefault().Id;
                            if (role == "Agent")
                            {
                                //topup luôn
                                logger.Info(string.Format("WEB ACTIVE TOPUP @Phone{0} @Amount{1}", _agency.PhoneNumber, agent_active.Amount));
                                string TOPUP = Utils.TOPUP.TopuptoUserId(_agency.PhoneNumber, agent_active.Amount.ToString());
                                logger.Info(TOPUP);
                                //lưu lại thông tin 
                                var _bonus = db.Agent_Bonus.SingleOrDefault(a => a.UserName == product.Active_by);
                                if (_bonus != null)
                                {
                                    _bonus.Used = _bonus.Used + agent_active.Amount;
                                    _bonus.Active = _bonus.Active + agent_active.Amount;
                                    _bonus.Newdate = DateTime.Now;
                                    db.Entry(_bonus).State = EntityState.Modified;
                                }
                                else
                                {
                                    var agent_bonus = new Agent_Bonus()
                                    {
                                        UserName = product.Active_by,
                                        Used = agent_active.Amount,
                                        Active = agent_active.Amount,
                                        Createdate = DateTime.Now,
                                        Newdate = DateTime.Now
                                    };
                                    db.Agent_Bonus.Add(agent_bonus);
                                }
                            }
                            else if (role == "Staff")
                            {
                                //lưu thông tin lại
                                //cộng tiền vào tài khoản cho staff
                                var _bonus = db.Agent_Bonus.SingleOrDefault(a => a.UserName == product.Active_by);
                                if (_bonus != null)
                                {
                                    _bonus.Active = _bonus.Active + agent_active.Amount;
                                    _bonus.Newdate = DateTime.Now;
                                    db.Entry(_bonus).State = EntityState.Modified;
                                }
                                else
                                {
                                    var agent_bonus = new Agent_Bonus()
                                    {
                                        UserName = product.Active_by,
                                        Active = agent_active.Amount,
                                        Createdate = DateTime.Now,
                                        Newdate = DateTime.Now
                                    };
                                    db.Agent_Bonus.Add(agent_bonus);
                                }
                            }
                        }

                    }
                    //thưởng cho khách hàng
                    msg = AddBonus(product.Model, product.Code, old_customer);

                    //luu thong tin vao db
                    db.SaveChanges();

                    //show thong tin kich hoat theo SerialBrand
                    var model = new ProductResult()
                    {
                        Name = product.Name,
                        Code = product.Code,
                        SerialBrand = product.SerialBrand,
                        Serial = product.Serial,
                        Model = product.Model,
                        Trademark = product.Trademark,
                        Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                        Limited = product.Limited.ToString(),
                        Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                        Bonus = msg
                    };

                    result.Status = 1;
                    result.Message = "Kích hoạt bảo hành thành công";
                    result.Data = new List<ProductResult>() { model };
                }

                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                result.Status = 1;
                result.Message = "Kích hoạt không thành công";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
        }
        string AddBonus(string Model, string ProductCode, Customer customer)
        {
            string msg = "";
            if (!string.IsNullOrEmpty(Model))
            {
                var bonus = db.B_Model_Process.Where(a => a.Model == Model && a.Status == true);
                if (bonus.Count() > 0)
                {
                    foreach (var item in bonus)
                    {
                        var process = db.B_Process.SingleOrDefault(a => a.Id == item.Process && a.Status == true);
                        if (process != null)
                        {
                            if (process.Startdate != null && process.Enddate != null)
                            {
                                //check hạn sử dụng chương trình
                                if (process.Startdate.Value.Date > DateTime.Now.Date)
                                {
                                    //chưa đến ngày được tham gia chương trình
                                    continue;
                                }
                                if (process.Enddate.Value.Date < DateTime.Now.Date)
                                {
                                    //hết ngày tham gia chương trình
                                    continue;
                                }
                            }
                            var log = new B_Log_Active()
                            {
                                Model = Model,
                                Process = process.Name,
                                ProcessId = item.Process,
                                ProductCode = ProductCode,
                                Unit = process.Unit,
                                Quantity = process.Quantity
                            };
                            db.B_Log_Active.Add(log);
                            if (process.Name == "TD")
                            {
                                customer.PointActive = customer.PointActive + process.Quantity;
                            }
                            msg = msg + " " + process.Quantity + " " + process.Unit + ",";

                        }
                    }
                    //db.SaveChanges();
                    return msg;
                }
            }
            return msg;
        }
        [Route("warranti")]
        [HttpPost]
        public HttpResponseMessage Warranti(WarrantiReq warranti)
        {
            var result = new ProductActiveRes();
            try
            {
                
                if (string.IsNullOrEmpty(warranti.Phone))
                {
                    result.Status = 1;
                    result.Message = "Nhập số điện thoại bảo hành";
                    result.Data = new List<ProductResult>();
                    return ResponseMessage(result);
                }
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    result.Status = 1;
                //    result.Message = "Số điện thoại không đúng";
                //    result.Data = new List<ProductResult>();
                //    return ResponseMessage(result);
                //}
                if (string.IsNullOrEmpty(warranti.Province) || string.IsNullOrEmpty(warranti.District) || string.IsNullOrEmpty(warranti.Ward) || string.IsNullOrEmpty(warranti.Address))
                {
                    result.Status = 1;
                    result.Message = "Nhập thông tin địa chỉ";
                    result.Data = new List<ProductResult>();
                    return ResponseMessage(result);
                }
                if (string.IsNullOrEmpty(warranti.Note))
                {
                    result.Status = 1;
                    result.Message = "Nhập thông tin mô tả lỗi";
                    result.Data = new List<ProductResult>();
                    return ResponseMessage(result);
                }
                //check thông tin sản phẩm
                var product = db.Products.FirstOrDefault(a => a.SerialBrand == warranti.ProductCode && a.Status == 1);
                if (product == null)
                {
                    result.Status = 1;
                    result.Message = "Sản phẩm không tồn tại trọng hệ thống.";
                    result.Data = new List<ProductResult>();
                    return ResponseMessage(result);
                }
                //check tạo 1 serial 1 ngày
                //check phieu da tao chua
                //var today = DateTime.Now.Date;

                //var _old = db.Warrantis.Where(a => a.ProductCode == warranti.ProductCode).OrderByDescending(a=>a.Createdate).FirstOrDefault();
                //if (_old!=null)
                //{
                //    var outdate = _old.Createdate.Value.AddDays(3).Date;
                //    if(outdate> today)
                //    {
                //        result.Status = 1;
                //        result.Message = "Serial sản phẩm đã được tạo phiếu bảo hành.";
                //        result.Data = new List<ProductResult>();
                //        return ResponseMessage(result);
                //    }                    
                //}
                //tao hoac update thong tin khach hang
                var old_customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (old_customer == null)
                {
                    //result.Status = 1;
                    //result.Message = "Số điện thoại không tồn tại trong hệ thống bảo hành.";
                    //result.Data = new List<ProductResult>();
                    //return ResponseMessage(result);
                    //taoj moi thong tin khacsh hang
                    var customer = new Customer()
                    {
                        Createdate = DateTime.Now,
                        Phone = warranti.Phone,
                        Chanel = warranti.Chanel,
                        Province = warranti.Province,
                        District = warranti.District,
                        Ward = warranti.Ward,
                        Address = warranti.Address,
                        Name = warranti.CusName,
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                else
                {
                    //update thong tin khach hang
                    if (string.IsNullOrEmpty(old_customer.Province))
                    {
                        old_customer.Name = warranti.CusName;
                        old_customer.Province = warranti.Province;
                        old_customer.District = warranti.District;
                        old_customer.Ward = warranti.Ward;
                        old_customer.Address = warranti.Address;

                        db.Entry(old_customer).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var _warranti = new Warranti();

                _warranti.Phone = warranti.Phone;
                _warranti.PhoneExtra = warranti.PhoneExtra;
                _warranti.Province = warranti.Province;
                _warranti.District = warranti.District;
                _warranti.Ward = warranti.Ward;
                _warranti.Address = warranti.Address;
                _warranti.Note = warranti.Note;                

                _warranti.ProductCode = warranti.ProductCode;
                _warranti.Serial = warranti.SerialHot;
                _warranti.Model = warranti.Model;
                _warranti.ProductName = warranti.ProductName;

                _warranti.Createby = warranti.Createby;
                _warranti.Createdate = DateTime.Now;
                _warranti.Status = 0;
                _warranti.Chanel = warranti.Chanel;

                if (User.IsInRole("Trạm - Trưởng trạm"))
                {
                    _warranti.Status = 2;
                    _warranti.Station_Warranti = User.Identity.Name;

                }
                db.Warrantis.Add(_warranti);
                db.SaveChanges();

                _warranti.Code = Utils.Control.CreateCode(_warranti.Id);
                db.Entry(_warranti).State = EntityState.Modified;

                var log = new Warranti_Log()
                {
                    Id_Warranti = _warranti.Id,
                    Createdate = DateTime.Now,
                    Description = User.Identity.Name + " Tạo mới phiếu bảo hành."
                };
                db.Warranti_Log.Add(log);
                db.SaveChanges();

                result.Status = 1;
                result.Message = "Đã tạo phiếu bảo hành thành công.";
                result.Data = new List<ProductResult>();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                result.Status = 1;
                result.Message = "Không tạo được phiếu bảo hành.";
                result.Data = new List<ProductResult>();
            }

            return ResponseMessage(result);

        }

        [Route("payment")]
        [HttpPost]
        public HttpResponseMessage Payment(Payment payment)
        {
            var result = new ResultString();
            try
            {
                var customer = db.Customers.SingleOrDefault(a => a.Phone == payment.Phone);
                if (customer != null)
                {
                    var cate = db.B_Cate.Find(payment.Process);
                    int Point = customer.PointActive - customer.PointPayment;
                    DateTime date = DateTime.Now.Date;
                    if (cate.Enddate != null)
                    {
                        if (cate.Enddate.Value.Date < date)
                        {
                            result.Status = 1;
                            result.Message = "CHƯƠNG TRÌNH ĐÃ HẾT HẠN SỬ DỤNG";
                            result.Data = new List<string>();
                            return ResponseMessage(result);
                        }
                    }
                    if (Point > 0 && Point >= cate.Point)
                    {
                        //trừ điểm của khách hang đi 
                        customer.PointPayment = customer.PointPayment + cate.Point;
                        db.Entry(customer).State = EntityState.Modified;
                        //trả thưởng cho khách hàng
                        if (cate.Code == "VOUCHER")
                        {
                            // lấy mã voucher trả cho khách
                            var voucher = db.B_Voucher.Where(a => a.Status == false & a.Amount == cate.Amount).FirstOrDefault();
                            if (voucher != null)
                            {
                                var pay = new B_Payment()
                                {
                                    Createdate = DateTime.Now,
                                    PayAmount = cate.Amount,
                                    PayCate = cate.Code,
                                    Phone = payment.Phone,
                                    Point = Point,
                                    Status = true,
                                    PointCharge = cate.Point,
                                    PayContent = voucher.Code
                                };
                                db.B_Payment.Add(pay);
                                voucher.Status = true;
                                voucher.Activedate = DateTime.Now;
                                voucher.PhoneActive = payment.Phone;
                                db.Entry(voucher).State = EntityState.Modified;
                                db.SaveChanges();

                                result.Status = 0;
                                result.Message = voucher.Code;
                                result.Data = new List<string>() { cate.Description };
                                return ResponseMessage(result);
                            }
                            else
                            {
                                result.Status = 1;
                                result.Message = "QUÀ TẶNG NÀY ĐÃ HẾT. VUI LÒNG CHỌN 1 QUÀ TẶNG KHÁC";
                                result.Data = new List<string>();
                                return ResponseMessage(result);
                            }
                        }
                        else if (cate.Code == "TOPUP")
                        {
                            string TOPUP = Utils.TOPUP.TopuptoUserId(payment.Phone, cate.Amount.ToString());
                            if (TOPUP == "0")
                            {
                                var pay = new B_Payment()
                                {
                                    Createdate = DateTime.Now,
                                    PayAmount = cate.Amount,
                                    PayCate = cate.Code,
                                    Phone = payment.Phone,
                                    Point = Point,
                                    Status = true,
                                    PointCharge = cate.Point,
                                    PayContent = "",
                                };
                                db.B_Payment.Add(pay);
                                db.SaveChanges();

                                result.Status = 1;
                                result.Message = "Đã TOPUP thành công " + cate.Amount + " đến thuê bao " + payment.Phone;
                                result.Data = new List<string>();
                                return ResponseMessage(result);
                            }
                            else
                            {
                                result.Status = 1;
                                result.Message = "LỖI TOPUP KHÔNG THÀNH CÔNG";
                                result.Data = new List<string>();
                                return ResponseMessage(result);
                            }
                        }
                    }
                }
                result.Status = 1;
                result.Message = "BẠN KHÔNG ĐỦ ĐIỀU KIỆN THAM GIA CHƯƠNG TRÌNH NÀY";
                result.Data = new List<string>();
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                result.Status = 1;
                result.Message = "CÓ LỖI NGOẠI LỆ. LIÊN HỆ NGAY ĐỂ ĐƯỢC XỬ LÝ";
                result.Data = new List<string>();
                return ResponseMessage(result);
            }
        }
        //get
        [Route("searchproduct")]
        [HttpGet]
        public HttpResponseMessage SearchProduct(string data)
        {
            var result = new ProductActiveRes();

            //search theo thông tin số điện thoại
            var actives = from a in db.Products
                                where a.Active_phone == data
                                orderby a.Active_date descending
                                select new ListActive()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    Model = a.Model,
                                    Code = a.Code,
                                    Limited = a.Limited,
                                    Active_date = a.Active_date,
                                    End_date = a.End_date,
                                };
            var listActiveFor = new List<ListActive>();
            listActiveFor = actives.ToList();
            if (listActiveFor.Count > 0)
            {
                result.Status = 2;
                result.Message = "Thông tin bảo hành sản phẩm";
                result.ListActives = listActiveFor;
                return ResponseMessage(result);
            }

            //search theo thông tin mã cào
            var product = db.Products.SingleOrDefault(a => a.Code == data);
            var today = DateTime.Now;
            today = today.AddDays(1);
            //thông tin check quá 35 ngày
            var date = DateTime.Now.AddDays(-35);
            
            if (product == null)
            {
                result.Status = 1;
                result.Message = "Sản phẩm không được tìm thấy trong hệ thống.";
                result.Data = new List<ProductResult>();
                return ResponseMessage(result);
            }
            else if(product.Status == 0 && product.Createdate < date)
            {
                //trường hợp chưa kích hoạt nhưng quá hạn
                var startdate = product.Createdate.Value.AddDays(35);
                var enddate = startdate.AddMonths(product.Limited);

                var model = new ProductResult()
                {
                    Name = product.Name,
                    Code = product.Code,
                    Model = product.Model,
                    Trademark = product.Trademark,
                    Activedate = startdate.ToString("dd/MM/yyyy"),
                    Limited = product.Limited.ToString(),
                    Enddate = enddate.ToString("dd/MM/yyyy"),
                };
                result.Status = 1;
                result.Message = "Thông tin bảo hành sản phẩm.";
                result.Data = new List<ProductResult>() { model };
                return ResponseMessage(result);
            }
            else if (product.End_date < today)
            {
                var model = new ProductResult()
                {
                    Name = product.Name,
                    Code = product.Code,
                    Model = product.Model,
                    Trademark = product.Trademark,
                    Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                    Limited = product.Limited.ToString(),
                    Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                };
                result.Status = 1;
                result.Message = "Sản phẩm đã quá hạn bảo hành.";
                result.Data = new List<ProductResult>() { model };
                return ResponseMessage(result);
            }
            else
            {
                var model = new ProductResult()
                {
                    Name = product.Name,
                    Code = product.Code,
                    Model = product.Model,
                    Trademark = product.Trademark,
                    Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                    Limited = product.Limited.ToString(),
                    Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                };
                result.Status = 1;
                result.Message = "Thông tin bảo hành sản phẩm.";
                result.Data = new List<ProductResult>() { model };
                return ResponseMessage(result);
            }
            
        }
        [Route("searchwarranti")]
        [HttpGet]
        public HttpResponseMessage SearchWarranti(string phone)
        {
            var _warr = new Warranti();
            var result = new WarrantiRes();
            var warranti = db.Warrantis.OrderByDescending(a => a.Createdate).Where(a => a.Phone == phone);
            if (warranti.Count() > 0)
            {
                _warr = warranti.FirstOrDefault();
            }
            else
            {
                var warr_serial = db.Warrantis.OrderByDescending(a => a.Createdate).Where(a => a.ProductCode == phone);
                _warr = warr_serial.FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(_warr.Code))
            {

                var model = new WarrantiModel()
                {
                    ProductCode = _warr.ProductCode,

                    Code = _warr.Code,
                    Status = Common.getStatusstring(_warr.Status),
                    Deadline = (_warr.Deadline != null) ? (_warr.Deadline.Value.ToString("dd/MM/yyyy")) : "",
                    Station_Warranti = _warr.Station_Warranti,
                    KTV_Warranti = _warr.KTV_Warranti,
                    Createdate = (_warr.Createdate != null) ? (_warr.Createdate.Value.ToString("dd/MM/yyyy")) : "",

                    Successdate = (_warr.Successdate != null) ? (_warr.Successdate.Value.ToString("dd/MM/yyyy")) : "",
                    Successnote = _warr.Successnote
                };

                result.Status = 1;
                result.Message = "Thông tin phiếu bảo hành mới nhất.";
                result.Data = new List<WarrantiModel>() { model };

            }
            else
            {
                result.Status = 1;
                result.Message = "Không tìm thấy thông tin bảo hành.";
                result.Data = new List<WarrantiModel>();
            }

            return ResponseMessage(result);
        }

        [Route("getproduct")]
        public HttpResponseMessage GetProduct(string code)
        {
            var result = new ProductRes();
            var product = db.Products.FirstOrDefault(a => a.SerialBrand == code);
            if (product != null)
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<Product>() { product };
                return ResponseMessage(result);
            }
            else
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<Product>();
                return ResponseMessage(result);
            }
        }
        [Route("getstamps")]
        public HttpResponseMessage GetStamps(string code)
        {
            var result = new ProductStampsRes();
            var productstamps = db.ProductStamps.FirstOrDefault(a => a.Code == code);
            if (productstamps != null)
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<ProductStamp>() { productstamps };
                return ResponseMessage(result);
            }
            else
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<ProductStamp>();
                return ResponseMessage(result);
            }
        }
        [Route("getprodbysrbrand")]
        public HttpResponseMessage GetProdBySrBrand(string serialBrand)
        {
            var result = new ProductRes();
            var product = db.Products.FirstOrDefault(a => a.SerialBrand == serialBrand);
            if (product != null)
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<Product>() { product };
                return ResponseMessage(result);
            }
            else
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<Product>();
                return ResponseMessage(result);
            }
        }
        [Route("getcustomer")]
        public HttpResponseMessage GetCustomer(string phone)
        {
            var customer = db.Customers.SingleOrDefault(a => a.Phone == phone);

            var result = new CustomerRes();
            if (customer == null)
            {
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<CustomerResult>();
            }
            else
            {
                var product = db.Products.Where(a => a.Active_phone == customer.Phone);

                var model = new CustomerResult()
                {
                    Name = customer.Name,
                    Province = customer.Province,
                    District = customer.District,
                    Ward = customer.Ward,
                    Address = customer.Address,
                    ListProduct = product.ToList()
                };
                result.Status = 1;
                result.Message = "OK";
                result.Data = new List<CustomerResult>() { model };
            }
            return ResponseMessage(result);
        }

        [Route("getprovince")]
        public HttpResponseMessage GetProvince()
        {
            var province = from a in db.Provinces
                           orderby a.Name
                           select a.Name;
            var result = new ResultString();

            result.Status = 1;
            result.Message = "OK";
            result.Data = province.ToList();

            return ResponseMessage(result);
        }
        [Route("getdistrict")]
        public HttpResponseMessage GetDistrict(string province)
        {
            var district = from a in db.Districts
                           where a.Province.Name.Equals(province)
                           orderby a.Name
                           select a.Name;
            var result = new ResultString();

            result.Status = 1;
            result.Message = "OK";
            result.Data = district.ToList();

            return ResponseMessage(result);
        }

        [Route("getward")]
        public HttpResponseMessage GetWard(string district)
        {
            var ward = from a in db.Wards
                       where a.District.Name.Equals(district)
                       orderby a.Name
                       select a.Name;
            var result = new ResultString();

            result.Status = 1;
            result.Message = "OK";
            result.Data = ward.ToList();

            return ResponseMessage(result);
        }
        //response
        private HttpResponseMessage ResponseMessage(Result result)
        {
            string json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }

        int Getpricemodel(string Model)
        {
            var model = db.Model_Price.SingleOrDefault(a => a.Model == Model);
            if (model != null)
            {
                return model.Price;
            }
            else
            {
                return 0;
            }
        }
    }
}
