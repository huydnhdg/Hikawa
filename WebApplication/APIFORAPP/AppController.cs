using ImageResizer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using NLog;
//using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using WebApplication.APIFORAPP.Model;
using WebApplication.APIFORAPP.Model_Order;
using WebApplication.APIFORAPP.Model_Teka;
using WebApplication.APIFORAPP.Model_Teka.Model_Request;
using WebApplication.APIFORAPP.ModelRequest;
using WebApplication.APIFORAPP.Product_Export;
using WebApplication.Areas.Admin.Data;
using WebApplication.FCM;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.APIFORAPP
{
    [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        HikawaEntities db = new HikawaEntities();
        Logger logger = LogManager.GetCurrentClassLogger();
        string domain = WebConfigurationManager.AppSettings["DOMAIN"];

        //response
        private HttpResponseMessage ResponseMessage(Result result)
        {
            string json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage();
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }

        //account
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<HttpResponseMessage> Login(LoginViewModel model)
        {
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password,
                model.RememberMe, shouldLockout: false);
            var res = new UserModelRes()
            {
                Status = 1,
                Message = "Đăng nhập thành công.",
                Data = new List<UserModel>()
            };
            switch (result)
            {
                case SignInStatus.Success:
                    try
                    {
                        var user = from a in db.AspNetUsers
                                   select new UserModel()
                                   {
                                       Id = a.Id,
                                       UserName = a.UserName,
                                       PhoneNumber = a.PhoneNumber,
                                       //Createdate = a.Createdate,
                                       Email = a.Email,
                                       Address = a.Address,
                                       //BankAccount = a.BankAccount,
                                       //BankAccountHolder = a.BankAccountHolder,
                                       //BankName = a.BankName,
                                       Nguoidaidien = a.FullName,
                                       //AgentName = a.AgentName,
                                       //Taxcode = a.TaxCode,
                                       Role = a.AspNetRoles.FirstOrDefault().Id.Equals("Agent") ? "Agency" : a.AspNetRoles.FirstOrDefault().Id
                                   };
                        res.Data = new List<UserModel>() { user.FirstOrDefault(a => a.UserName == model.UserName) };
                    }
                    catch (Exception ex)
                    {
                        logger.Info(ex);
                    }

                    return ResponseMessage(res);
                case SignInStatus.LockedOut:
                    res.Status = -1;
                    res.Message = "Tài khoản đã bị khóa.";
                    return ResponseMessage(res);
                case SignInStatus.Failure:
                default:
                    res.Status = -1;
                    res.Message = "Đăng nhập không thành công";
                    return ResponseMessage(res);
            }
        }
        [Route("logout")]
        [HttpPost]
        public HttpResponseMessage Logout()
        {
            IAuthenticationManager AuthenticationManager = Request.GetOwinContext().Authentication;
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            var res = new UserModelRes()
            {
                Status = 1,
                Message = "Đăng xuất thành công.",
                Data = new List<UserModel>()
            };
            return ResponseMessage(res);
        }
        [Route("changepassword")]
        [HttpPost]
        public async Task<HttpResponseMessage> ChangePassword(ChangePasswordViewModel model)
        {
            var user = db.AspNetUsers.Where(a => a.UserName == model.UserName).SingleOrDefault();
            if (user != null)
            {
                var result = await UserManager.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var usermanage = await UserManager.FindByIdAsync(user.Id);
                    if (usermanage != null)
                    {
                        await SignInManager.SignInAsync(usermanage, isPersistent: false, rememberBrowser: false);
                    }
                    var res = new UserModelRes()
                    {
                        Status = 1,
                        Message = "Đổi mật khẩu thành công.",
                        Data = new List<UserModel>()
                    };
                    return ResponseMessage(res);
                }
            }

            var resul = new UserModelRes()
            {
                Status = -1,
                Message = "Đổi mật khẩu không thành công.",
                Data = new List<UserModel>()
            };
            return ResponseMessage(resul);
        }
        [Route("postdevice")]
        [HttpPost]
        public HttpResponseMessage PostDevice(User_Device user)
        {
            try
            {
                var userdevice = db.User_Device.Where(a => a.UserName == user.UserName).SingleOrDefault();
                if (userdevice == null)
                {
                    user.Createdate = DateTime.Now;
                    db.User_Device.Add(user);
                    db.SaveChanges();
                    var result = new ResultString()
                    {
                        Status = 1,
                        Message = "OK",
                        Data = new List<string>()
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    userdevice.DeviceId = user.DeviceId;
                    userdevice.Provider = user.Provider;
                    userdevice.Createdate = DateTime.Now;
                    db.Entry(userdevice).State = EntityState.Modified;
                    db.SaveChanges();
                    var result = new ResultString()
                    {
                        Status = 1,
                        Message = "OK",
                        Data = new List<string>()
                    };
                    return ResponseMessage(result);
                }
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                var result = new ResultString()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
        }

        //get 

        //get banner & article
        [Route("getbanners")]
        [HttpGet]
        public HttpResponseMessage GetBanner()
        {
            var model = db.Articles.OrderByDescending(a => a.Createdate).FirstOrDefault(a => a.Cate == "banner");
            var ar = new Banner()
            {
                Image = ConfigControl.DOMAIN + model.Image,
                Title = model.Title,
                Link = model.Link
            };
            var result = new TK_Banner()
            {
                Status = 1,
                Message = "OK",
                Data = new List<Banner>() { ar }
            };
            return ResponseMessage(result);

        }
        [Route("getarticles")]
        [HttpGet]
        public HttpResponseMessage GetArticle(string url = "")
        {

            var model = from a in db.Articles
                        where a.Cate != "banner"
                        select new TK_Article()
                        {
                            Title = a.Title,
                            Description = a.Description,
                            Text = a.Detail,
                            CreateDate = a.Createdate,
                            Url = a.Link,
                            Image = ConfigControl.DOMAIN + a.Image

                        };
            TK_Article artice = new TK_Article();
            if (!string.IsNullOrEmpty(url))
            {
                artice = model.Where(a => a.Url == url).SingleOrDefault();
                artice.Text = artice.Text.Replace("/Data/images/", domain + "Data/images/");
                var res = new ArticleRes()
                {
                    Status = 1,
                    Message = "Danh sách tin tức",
                    Data = new List<TK_Article>() { artice }
                };
                return ResponseMessage(res);
            }
            var result = new ArticleRes()
            {
                Status = 1,
                Message = "Danh sách tin tức",
                Data = model.ToList()
            };
            return ResponseMessage(result);

        }
        [Route("getlistcate")]
        [HttpGet]
        public HttpResponseMessage GetCateArticle()
        {

            var model = from a in db.Article_Cate
                        select new Article_Cate_Model()
                        {
                            Cate = a.Cate,
                            Title = a.Name,
                            CreateDate = a.Createdate,
                            Image = ConfigControl.DOMAIN + a.Image

                        };
            model = model.Where(a => a.Title != "Banner");
            var res = new Model_Teka.Article_Cate()
            {
                Status = 1,
                Message = "Danh sách phân loại",
                Data = model.ToList()
            };
            return ResponseMessage(res);

        }
        [Route("getlistarticle")]
        [HttpGet]
        public HttpResponseMessage GetListArticle(string cate = "")
        {

            var model = from a in db.Articles
                        select new TK_Article()
                        {
                            Title = a.Title,
                            Description = a.Description,
                            Text = a.Detail,
                            CreateDate = a.Createdate,
                            Url = a.Link,
                            Image = ConfigControl.DOMAIN + a.Image,
                            Cate = a.Cate

                        };
            model = model.Where(a => a.Cate != "Banner");
            if (!string.IsNullOrEmpty(cate))
            {
                model = model.Where(a => a.Cate == cate);
            }
            var res = new ArticleRes()
            {
                Status = 1,
                Message = "Danh sách bài viết",
                Data = model.ToList()
            };
            return ResponseMessage(res);

        }

        //address        
        [Route("province")]
        [HttpGet]
        public HttpResponseMessage GetProvince(string textSearch = "")
        {
            var model = from a in db.Provinces
                        orderby a.Name
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new ProvinceRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("district")]
        [HttpGet]
        public HttpResponseMessage GetDistrict(long Id, string textSearch = "")
        {
            var model = from a in db.Districts
                        orderby a.Name
                        where a.ProvinceId == Id
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new DistrictRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("ward")]
        [HttpGet]
        public HttpResponseMessage GetWard(long Id, string textSearch = "")
        {
            var model = from a in db.Wards
                        orderby a.Name
                        where a.DistrictID == Id
                        select new Adr()
                        {
                            Id = a.Id,
                            Name = a.Name
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch));
            }
            var result = new WardRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        //get tem cào
        [Route("getcodestamps")]
        [HttpGet]
        public HttpResponseMessage GetCodeStamps(string code)
        {
            var result = new CodeStampsRes();
            //check mã cào
            var actived = db.ProductStamps.FirstOrDefault(c => c.Code == code);
            if (actived != null)
            {
                if (actived.Status == 0)
                {
                    result = new CodeStampsRes()
                    {
                        Status = 0,
                        Message = "Tem cào chưa kích hoạt",
                        Data = actived,
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    result = new CodeStampsRes()
                    {
                        Status = 1,
                        Message = "Tem cào đã được kích hoạt",
                        Data = null,
                    };
                    return ResponseMessage(result);
                }
            }
            else
            {
                result = new CodeStampsRes()
                {
                    Status = 2,
                    Message = "Tem cào không tồn tại",
                    Data = null,
                };
                return ResponseMessage(result);
            }

            //return ResponseMessage(result);
        }
        //get product
        [Route("getproduct")]
        [HttpGet]
        public HttpResponseMessage GetProduct(string serial)
        {
            var result = new TK_Prodres();
            // lấy danh sách sản phẩm đã kích hoạt theo Sđt
            var actives = from a in db.Products
                          where a.Active_phone == serial
                          orderby a.Active_date descending
                          select new TK_Product()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Model = a.Model,
                              Code = a.Code,
                              Limited = a.Limited,
                              Activedate = a.Active_date,
                              EndDate = a.End_date,
                          };
            var products = new List<TK_Product>();
            products = actives.ToList();
            if (products.Count > 0)
            {
                result = new TK_Prodres()
                {
                    Status = 2,
                    Message = "Danh sách sản phẩm đã được kích hoạt trước đó",
                    Data = products,
                };
                return ResponseMessage(result);
            }

            // lấy thông tin sản phẩm theo Serial Brand
            var model = db.Products.Where(a => a.SerialBrand == serial).FirstOrDefault();
            if (model == null)
            {
                // Sản phẩm không tồn tại
                result = new TK_Prodres()
                {
                    Status = -1,
                    Message = "Sản phẩm không tồn tại.",
                    Data = new List<TK_Product>()
                };
            }
            else
            {
                // 
                if (model.Status == 1)
                {
                    // Sản phẩm đã được kích hoạt
                    var product = new TK_Product()
                    {
                        Activedate = model.Active_date,
                        //Buydate = model.Buydate,
                        //BuyAdr = model.NameAgency, // Tên Đại lý
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Serial,
                        Model = model.Model,
                        Serial = model.Code,
                        Limited = model.Limited,
                        EndDate = model.End_date,
                        PhoneActive = model.Active_phone,
                    };
                    result = new TK_Prodres()
                    {
                        Status = 1,
                        Message = "Sản phẩm đã được kích hoạt",
                        Data = new List<TK_Product>() { product },
                    };
                }
                else
                {
                    //chua kich hoat
                    var product = new TK_Product()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Code = model.Serial,
                        Model = model.Model,
                        Serial = model.Code,
                        Limited = model.Limited,

                    };
                    result = new TK_Prodres()
                    {
                        Status = 0,
                        Message = "Sản phẩm chưa được kích hoạt.",
                        Data = new List<TK_Product>() { product },
                    };

                }
            }
            return ResponseMessage(result);
        }

        [Route("getcustomer")]
        [HttpGet]
        public HttpResponseMessage GetCustomer(string phone)
        {
            try
            {
                var customer = db.Customers.SingleOrDefault(a => a.Phone == phone);
                if (customer != null)
                {
                    var product = db.Products.Where(a => a.Active_phone == customer.Phone);
                    var model = new CustomerModel()
                    {
                        Id = customer.Id,
                        Phone = customer.Phone,
                        Name = customer.Name,
                        Address = customer.Address,
                        Ward = customer.Ward,
                        District = customer.District,
                        City = customer.Province,
                        ListProduct = product.ToList()
                    };
                    var resultss = new CustomerResRenew()
                    {
                        Status = 1,
                        Message = "OK",
                        Data = new List<CustomerModel>() { model },
                    };
                    return ResponseMessage(resultss);
                }
                var result = new CustomerResRenew()
                {
                    Status = -1,
                    Message = "Số điện thoại chưa có trong hệ thống.",
                    Data = new List<CustomerModel>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new CustomerResRenew()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<CustomerModel>()
                };
                return ResponseMessage(result);
            }

        }

        [Route("getactive")]
        [HttpGet]
        public HttpResponseMessage GetActive(string serial)
        {
            try
            {
                var date = DateTime.Now.AddDays(-35);
                var today = DateTime.Now;
                today = today.AddDays(1);

                var product = db.Products.Where(a => a.SerialBrand == serial).FirstOrDefault();
                if (product == null)
                {
                    var res = new ActiveRes()
                    {
                        Status = -1,
                        Message = "Sản phẩm không tồn tại trong hệ thống",
                        Data = new List<ActiveModel>() { }
                    };
                    return ResponseMessage(res);
                }
                else if (product.Status == 0 && product.Createdate < date)
                {

                    //trường hợp chưa kích hoạt nhưng quá hạn
                    var startdate = product.Createdate.Value.AddDays(35);
                    var enddate = startdate.AddMonths(product.Limited);
                    var model = new ActiveModel()
                    {
                        CusName = product.AgentC1,
                        Phone = "",
                        Address = "",
                        SerialBrand = product.SerialBrand,
                        Limited = product.Limited,
                        Activedate = startdate.ToString("dd/MM/yyyy"),
                        Enddate = enddate.ToString("dd/MM/yyyy"),
                        Buydate = product.Buydate != null ? product.Buydate.Value.ToString("dd/MM/yyyy") : ""
                    };

                    var res = new ActiveRes()
                    {
                        Status = 1,
                        Message = "Thông tin bảo hành sản phẩm",
                        Data = new List<ActiveModel>() { model }
                    };
                    return ResponseMessage(res);
                }
                else if (product.End_date < today)
                {
                    var customer = db.Customers.SingleOrDefault(a => a.Phone == product.Active_phone);
                    var model = new ActiveModel()
                    {
                        CusName = customer.Name,
                        Phone = customer.Phone,
                        Address = customer.Address + " " + customer.Ward + " " + customer.District + " " + customer.Province,
                        SerialBrand = product.SerialBrand,
                        Limited = product.Limited,
                        Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                        Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                        Buydate = product.Buydate != null ? product.Buydate.Value.ToString("dd/MM/yyyy") : ""
                    };

                    var res = new ActiveRes()
                    {
                        Status = 1,
                        Message = "Sản phẩm đã quá hạn bảo hành",
                        Data = new List<ActiveModel>() { model }
                    };
                    return ResponseMessage(res);
                }
                else
                {
                    var customer = db.Customers.SingleOrDefault(a => a.Phone == product.Active_phone);
                    var model = new ActiveModel()
                    {
                        CusName = customer.Name,
                        Phone = customer.Phone,
                        Address = customer.Address + " " + customer.Ward + " " + customer.District + " " + customer.Province,
                        SerialBrand = product.SerialBrand,
                        Limited = product.Limited,
                        Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                        Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                        Buydate = product.Buydate != null ? product.Buydate.Value.ToString("dd/MM/yyyy") : ""
                    };

                    var res = new ActiveRes()
                    {
                        Status = 1,
                        Message = "Thông tin bảo hành sản phẩm",
                        Data = new List<ActiveModel>() { model }
                    };
                    return ResponseMessage(res);
                }
            }
            catch (Exception ex)
            {
                var res = new ActiveRes()
                {
                    Status = -1,
                    Message = "Sản phẩm không tồn tại.",
                    Data = new List<ActiveModel>()
                };
                return ResponseMessage(res);
            }

        }

        [Route("getlistactive")]
        [HttpGet]
        public HttpResponseMessage GetListActive(string UserId = "", string str_date = "", string end_date = "", string serial = "", string status = "")
        {
            try
            {
                var cr_user = db.AspNetUsers.Find(UserId);
                var model = from a in db.Products
                            where a.Active_by == cr_user.UserName
                            join b in db.Customers on a.Active_phone equals b.Phone
                            join d in db.Agent_Log_Active on a.SerialBrand equals d.Product into temp
                            from ct in temp.DefaultIfEmpty()
                                //join c in db.Agent_Bonus on a.Active_by equals c.UserName into temp
                                //from ct in temp.DefaultIfEmpty()
                            select new ActiveListModel()
                            {
                                CusName = a.Name,
                                Phone = a.Active_phone,
                                Address = b.Address + " " + b.Ward + " " + b.District + " " + b.Province,
                                SerialBrand = a.SerialBrand,
                                Activedate = a.Active_date,
                                Amount = ct.Amount
                                //AgentId = b.Id,
                                //Amount = ct.Active
                            };

                //model = model.Where(a => a.AgentId == UserId);
                if (!string.IsNullOrEmpty(str_date))
                {
                    var from_date_str = DateTime.Parse(str_date, new System.Globalization.CultureInfo("pt-BR"));
                    from_date_str = from_date_str.AddDays(-1);
                    model = model.Where(da => da.Activedate > from_date_str);
                }
                if (!string.IsNullOrEmpty(end_date))
                {
                    var to_date_str = DateTime.Parse(end_date, new System.Globalization.CultureInfo("pt-BR"));
                    to_date_str = to_date_str.AddDays(1);
                    model = model.Where(da => da.Activedate < to_date_str);
                }
                if (!string.IsNullOrEmpty(serial))
                {
                    model = model.Where(a => a.SerialBrand == serial);
                }
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "0")
                    {
                        model = model.Where(a => a.Thanhtoan == null);
                    }
                    else if (status == "1")
                    {
                        model = model.Where(a => a.Thanhtoan != null);
                    }
                }
                var result = new ActiveListRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = model.ToList()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ActiveListRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<ActiveListModel>()
                };
                return ResponseMessage(result);
            }


        }

        [Route("getwarranti")]
        [HttpGet]
        public HttpResponseMessage GetWarranti(string serial = "", string phone = "")
        {
            try
            {
                var model = from w in db.Warrantis
                            select w;
                var list = model.ToList();
                if (!string.IsNullOrEmpty(serial))
                {
                    model = model.Where(w => w.ProductCode == serial);
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    model = model.Where(w => w.Phone == phone || w.PhoneExtra == phone);
                }
                if (model.Count() > 0)
                {
                    var a = model.OrderByDescending(w => w.Createdate).FirstOrDefault();
                    var data = new WarrantiModelReNew()
                    {
                        Id = a.Id,
                        SerialBrand = a.ProductCode,
                        Phone = a.Phone,
                        Phone2 = a.PhoneExtra,
                        //CusName = CusName,
                        Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                        Note = a.Note,
                        Status = a.Status,
                        Createdate = a.Createdate,
                        Createby = a.Createby,
                        KeyWarranti = a.Station_Warranti,
                        KTV = a.KTV_Warranti,
                        ProductCate = a.Cate,
                        Model = a.Model,
                        CodeWarr = a.Code,
                        LogWarrantis = db.Warranti_Log.Where(l => l.Id_Warranti == a.Id).ToList()
                    };

                    var result = new SearchWarrantiRes()
                    {
                        Status = 1,
                        Message = "OK",
                        Data = new List<WarrantiModelReNew>() { data }
                    };
                    return ResponseMessage(result);
                }
                else
                {
                    var result = new SearchWarrantiRes()
                    {
                        Status = 1,
                        Message = "Không tìm thấy thông tin phiếu bảo hành",
                        Data = new List<WarrantiModelReNew>()
                    };
                    return ResponseMessage(result);
                }

            }
            catch (Exception ex)
            {
                var result = new SearchWarrantiRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<WarrantiModelReNew>()
                };
                return ResponseMessage(result);
            }

        }

        [Route("getlistwarranti")]
        [HttpGet]
        public HttpResponseMessage GetListWarranti(string UserId = "", string str_date = "", string end_date = "", int status = -1, string textSearch = "", int page = 1)
        {
            try
            {
                var model = from a in db.Warrantis
                            join b in db.Customers on a.Phone equals b.Phone
                            select new WarrantiModelReNew()
                            {
                                Id = a.Id,
                                SerialBrand = a.ProductCode,
                                Phone = a.Phone,
                                Phone2 = a.Phone,
                                CusName = b.Name,
                                Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                                Note = a.Note,
                                Status = a.Status,
                                Createdate = a.Createdate,
                                Createby = a.Createby,
                                IdKey = a.Station_Warranti,
                                KeyWarranti = a.Station_Warranti,
                                IdKTV = a.KTV_Warranti,
                                KTV = a.KTV_Warranti,
                                ProductCate = a.Cate,
                                Model = a.Model,
                                CodeWarr = a.Code,
                                LogWarrantis = db.Warranti_Log.Where(l => l.Id_Warranti == a.Id).ToList()
                            };
                if (!string.IsNullOrEmpty(textSearch))
                {
                    model = model.Where(a => a.ProductCate.Contains(textSearch) || a.Model.Contains(textSearch)
                    || a.CodeWarr.Contains(textSearch) || a.SerialBrand.Contains(textSearch) || a.Phone.Contains(textSearch) || a.Phone2.Contains(textSearch));
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    var user = db.AspNetUsers.Find(UserId);
                    if (user.AspNetRoles.FirstOrDefault().Id == "Admin")
                    {

                    }
                    if (user.AspNetRoles.FirstOrDefault().Id == "Key")
                    {
                        model = model.Where(a => a.IdKey == user.UserName);
                    }
                    else if (user.AspNetRoles.FirstOrDefault().Id == "KTV")
                    {
                        model = model.Where(a => a.IdKTV == user.UserName);
                    }
                }
                if (status != -1)
                {
                    model = model.Where(a => a.Status == status);
                }
                if (!string.IsNullOrEmpty(str_date))
                {
                    var from_date_str = DateTime.Parse(str_date, new System.Globalization.CultureInfo("pt-BR"));
                    from_date_str = from_date_str.AddDays(-1);
                    model = model.Where(da => da.Createdate > from_date_str);
                }
                if (!string.IsNullOrEmpty(end_date))
                {
                    var to_date_str = DateTime.Parse(end_date, new System.Globalization.CultureInfo("pt-BR"));
                    to_date_str = to_date_str.AddDays(1);
                    model = model.Where(da => da.Createdate < to_date_str);
                }
                int pageSize = 10;
                int currentPage = page - 1;

                var result = new SearchWarrantiRes()
                {
                    Status = 1,
                    Message = "OK",
                    Data = model.OrderByDescending(a => a.Createdate).Skip(currentPage * pageSize).Take(10).ToList()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new SearchWarrantiRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<WarrantiModelReNew>()
                };
                return ResponseMessage(result);
            }
        }

        [Route("getlistkeywarranti")]
        [HttpGet]
        public HttpResponseMessage GetListKeyWarranti(string textSearch = "")
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where b.Id == "Key"
                        select new UserModel()
                        {
                            Id = a.UserName,
                            UserName = a.UserName,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("getlistktv")]
        [HttpGet]
        public HttpResponseMessage GetListKTV(string UserId, string textSearch = "")
        {
            var station = db.AspNetUsers.Find(UserId);
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where a.Createby == station.UserName
                        select new UserModel()
                        {
                            Id = a.UserName,
                            UserName = a.UserName,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("getlistagent")]
        [HttpGet]
        public HttpResponseMessage GetListAgent(string UserId, string textSearch = "")
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        where b.Id == "Agent"
                        select new UserModel()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                            AgentName = a.FullName,
                            PhoneNumber = a.PhoneNumber,
                            Createdate = a.Createdate,
                            Email = a.Email,
                            Address = a.Address,
                            Ward = a.Ward,
                            District = a.District,
                            Province = a.Province,
                        };
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.UserName.Contains(textSearch));
            }
            var result = new ListKeyRes()
            {
                Status = 1,
                Message = "Ok",
                Data = model.OrderBy(x => x.UserName).ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("getlogwarr")]
        [HttpGet]
        public HttpResponseMessage GetLogWarr(long id)
        {
            var log = db.Warranti_Log.Where(x => x.Id_Warranti == id).OrderByDescending(x => x.Createdate);
            var result = new LogWarrRes()
            {
                Status = 1,
                Message = "OK",
                Data = log.ToList(),
            };
            return ResponseMessage(result);
        }

        [Route("category")]
        [HttpGet]
        public HttpResponseMessage GetCategory(string textSearch = "")
        {
            List<string> list = new List<string>();
            list.Add("Bảo hành");
            list.Add("Lắp đặt");
            list.Add("Tính phí");
            list.Add("Bảo trì, bảo dưỡng");
            var result = new ResultString()
            {
                Status = 1,
                Message = "OK",
                Data = list
            };
            return ResponseMessage(result);
        }

        [Route("productcate")]
        [HttpGet]
        public HttpResponseMessage GetProductCate(string textSearch = "")
        {
            var cate = from a in db.ProductCates
                       select a;
            if (!string.IsNullOrEmpty(textSearch))
            {
                cate = cate.Where(a => a.Name.Contains(textSearch));
            }
            var result = new ResultString()
            {
                Status = 1,
                Message = "OK " + cate.Count(),
                Data = cate.Select(a => a.Name).ToList()
            };
            return ResponseMessage(result);
        }
        [Route("model")]
        [HttpGet]
        public HttpResponseMessage GetModel(string cate, string textSearch = "")
        {

            var result = new ResultString()
            {
                Status = 1,
                Message = "OK",
                Data = new List<string>()
            };
            return ResponseMessage(result);
        }

        [Route("getdeviceprice")]
        [HttpGet]
        public HttpResponseMessage GetDevicePrice(string keywarranti = "", string textSearch = "")
        {
            //lấy list linh kiện
            //lúc đẩy trạm lên có tên trạm nên phải tách chuỗi
            string username = "";
            if (!string.IsNullOrEmpty(keywarranti))
            {
                string[] word = keywarranti.Split(' ');
                username = word[0];
            }
            var cr_user = db.AspNetUsers.SingleOrDefault(a => a.UserName == username);
            var model = from a in db.Accessary_Big
                        select new PhutungPriceModel()
                        {
                            //có khả năng lỗi ID
                            //ID = a.Id,
                            Name = a.Name,
                            Code = a.Code,
                            Price = a.KeyPrice,
                            Cate = a.Code,
                            //Phanloai = a.Id_Key
                        };
            
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Name.Contains(textSearch) || a.Code.Contains(textSearch));
            }
            var df = new PhutungPriceModel()
            {
                Name = "Tư vấn khách hàng",
                Cate = "Tư vấn khách hàng",
                Phanloai = username,
                Code = "Tư vấn khách hàng",
            };
            var list = new List<PhutungPriceModel>();
            list.Add(df);
            list.AddRange(model.ToList());
            var result = new DevicePriceRes()
            {
                Status = 1,
                Message = "OK",
                Data = list
            };
            return ResponseMessage(result);
        }

        [Route("getmovefee")]
        [HttpGet]
        public HttpResponseMessage GetMoveFee()
        {
            var model = from a in db.Move_Price
                        select new MoveFee()
                        {
                            ID = a.Id,
                            Cate = a.Name,
                            Price = a.Price,
                            Note = a.Description
                        };
            var result = new MoveFeeRes()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }

        [Route("getwarrantidetail")]
        [HttpGet]
        public HttpResponseMessage GetDetail(long Id)
        {
            var warranti = db.Warrantis.Find(Id);
            var customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);

            var lsImage = db.Warranti_Image.Where(a => a.IdWarranti == warranti.Id);
            var lsDevice = from a in db.Warranti_Accessary
                           where a.IdWarranti == Id
                           select new PhuTung()
                           {
                               ID = a.Id.ToString(),
                               IDWarranti = a.IdWarranti,
                               IDPhutung = "",
                               Name = a.Name,
                               Quantity = a.Quantity,
                               Amount = a.Amount,
                               Price = a.Price,

                           };
            var st = db.AspNetUsers.SingleOrDefault(a => a.UserName == warranti.Station_Warranti);
            var kt = db.AspNetUsers.SingleOrDefault(a => a.UserName == warranti.KTV_Warranti);
            string station_name = "";
            string ktv_name = "";
            string ktv_phone = "";
            if (st != null)
            {
                station_name = st.FullName;
            }
            if (kt != null)
            {
                ktv_name = kt.FullName;
                ktv_phone = kt.PhoneNumber;
            }

            var model = new UpdateDetailReq()
            {
                Id = warranti.Id,
                SerialBrand = warranti.ProductCode,
                Phone = warranti.Phone + " " + warranti.Phone,
                Address = warranti.Address + " " + warranti.Ward + " " + warranti.District + " " + warranti.Province,
                CusName = customer.Name,
                Details = warranti.Note,
                Createdate = warranti.Createdate,

                KeyWarranti = warranti.Station_Warranti + " " + station_name,
                PhoneKTV = ktv_phone,
                KTV = warranti.KTV_Warranti + " " + ktv_name,


                ProductCate = warranti.Warranti_Cate,
                Model = warranti.Model,
                FeeSuggest = warranti.Price_Accessary,
                CheckIns = lsImage.Where(a => a.Type == 1).Select(a => a.Image).ToList(),
                Long = warranti.Long,
                Lat = warranti.Lat,
                Note = warranti.Note,
                Devices = lsDevice.ToList(),
                CheckOuts = lsImage.Where(a => a.Type == 2).Select(a => a.Image).ToList(),
                Price = warranti.Price,
                KM = warranti.KM,
                MoveFee = warranti.Price_Move,
                Amount = warranti.Amount,
                Sign = warranti.Sign

            };
            var result = new DetailWarrantiRes()
            {
                Status = 1,
                Message = "OK",
                Data = new List<UpdateDetailReq>() { model }
            };
            return ResponseMessage(result);
        }

        [Route("getreportactive")]
        [HttpGet]
        public HttpResponseMessage GetReportActive(string UserId = "", int year = 0)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var us = db.AspNetUsers.Find(UserId);
            var model = from a in db.Products
                        where a.Active_by == us.UserName && a.Status == 1
                        select a;
            int countProd = model.Count();
            int countAmount = 0;

            //check theo nam
            if (year > 2000)
            {
                model = model.Where(a => a.Active_date.Value.Year == year);
            }
            List<int> month = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                int item = model.Where(a => a.Active_date.Value.Month == i).Count();
                month.Add(item);
            }
            var report = new ReportImport()
            {
                Amount = countAmount,
                CountProduct = countProd,
                Month = month
            };
            var result = new ReportImportRes()
            {
                Status = 1,
                Message = "OK",
                Data = new List<ReportImport>() { report }
            };
            return ResponseMessage(result);

        }

        //post
        [Route("addkeywarr")]
        [HttpPost]
        public HttpResponseMessage AddKeyWarr(AddKeyWarrReq model)
        {
            string json = JsonConvert.SerializeObject(model);
            logger.Info(json);
            var warranti = db.Warrantis.Find(model.IDWarranti);
            //var user = db.AspNetUsers.Find(model.KeyWarranti);
            if (model.Status == 2)
            {
                var log = new Warranti_Log()
                {
                    Createdate = DateTime.Now,
                    Description = "Chuyển cho trạm bảo hành: " + model.KeyWarranti,
                    Id_Warranti = warranti.Id
                };
                db.Warranti_Log.Add(log);

                //luu thông tin vao db

                warranti.Station_Warranti = model.KeyWarranti;
                warranti.Deadline = model.Deadline;

                warranti.Status = 2;
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();//<----------------

                //gửi notifi cho trạm
                var sent = new SentNotify();
                sent.Sent(warranti.Station_Warranti, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.Code));

                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Chuyển trạm thành công.",
                    Data = new List<String>()

                };
                return ResponseMessage(result);
            }
            else
            {
                var log = new Warranti_Log()
                {
                    Createdate = DateTime.Now,
                    Description = "Thu hồi phiếu bảo hành của trạm " + warranti.Station_Warranti,
                    Id_Warranti = warranti.Id
                };
                db.Warranti_Log.Add(log);

                warranti.Station_Warranti = null;
                warranti.Deadline = null;
                warranti.Status = 0;
                db.Entry(warranti).State = EntityState.Modified;
                db.SaveChanges();//<----------------
                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Đã thu hồi phiếu bảo hành.",
                    Data = new List<String>()
                };
                return ResponseMessage(result);
            }

        }

        [Route("addktv")]
        [HttpPost]
        public HttpResponseMessage AddKTV(AddKTVReq model)
        {
            try
            {
                var warranti = db.Warrantis.Find(model.IDWarranti);
                if (warranti.KTV_Warranti != null)
                {
                    var resultEr = new ResultString()
                    {
                        Status = -1,
                        Message = "Phiếu đã được tiếp nhận bởi " + warranti.KTV_Warranti,
                        Data = new List<string>()
                    };
                    return ResponseMessage(resultEr);
                }
                //add ktv
                warranti.KTV_Warranti = model.IDKTV;
                warranti.Status = 5;//dang xu ly
                db.Entry(warranti).State = EntityState.Modified;

                //luu log trang thai
                var log = new Warranti_Log()
                {
                    Createdate = DateTime.Now,
                    Description = "Chuyển phiếu cho KTV: " + warranti.KTV_Warranti,
                    Id_Warranti = model.IDWarranti
                };
                db.Warranti_Log.Add(log);
                db.SaveChanges();
                //gửi notifi cho ktv
                var sent = new SentNotify();
                sent.Sent(warranti.KTV_Warranti, string.Format("Bạn nhận được 1 yêu cầu xử lý cho phiếu có mã {0}", warranti.Code));
                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Đã chuyển phiếu thành công cho KTV.",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }

        }

        [Route("warranti")]
        [HttpPost]
        public HttpResponseMessage Warranti(TK_Warranti warranti)
        {
            try
            {

                if (string.IsNullOrEmpty(warranti.Phone))
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Nhập số điện thoại bảo hành",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                //if (Utils.Control.getMobileOperator(warranti.Phone) == "UNKNOWN")
                //{
                //    var resulter = new ResultString()
                //    {
                //        Status = -1,
                //        Message = "Số điện thoại không đúng",
                //        Data = new List<string>()
                //    };
                //    return ResponseMessage(resulter);
                //}
                if (string.IsNullOrEmpty(warranti.Province) || string.IsNullOrEmpty(warranti.District) || string.IsNullOrEmpty(warranti.Ward) || string.IsNullOrEmpty(warranti.Address))
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Nhập thông tin địa chỉ",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                if (string.IsNullOrEmpty(warranti.Note))
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Nhập thông tin mô tả",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                //check tạo 1 serial 1 ngày
                //check phieu da tao chua
                //var today = DateTime.Now.Date;

                //var _old = db.Warrantis.Where(a => a.ProductCode == warranti.Serial).OrderByDescending(a => a.Createdate).FirstOrDefault();
                //if (_old != null)
                //{
                //    var outdate = _old.Createdate.Value.AddDays(3).Date;
                //    if (outdate > today)
                //    {
                //        var resulter = new ResultString()
                //        {
                //            Status = -1,
                //            Message = "Serial sản phẩm đã được tạo phiếu bảo hành.",
                //            Data = new List<string>()
                //        };
                //        return ResponseMessage(resulter);
                //    }

                //}

                //check thông tin sản phẩm
                //var product = db.Products.FirstOrDefault(a => a.Code == warranti.Serial && a.Status == 1);
                //if (product == null)
                //{
                //    var resulter = new ResultString()
                //    {
                //        Status = -1,
                //        Message = "Sản phẩm không tồn tại trọng hệ thống.",
                //        Data = new List<string>()
                //    };
                //    return ResponseMessage(resulter);
                //}
                //tao hoac update thong tin khach hang
                var old_customer = db.Customers.SingleOrDefault(a => a.Phone == warranti.Phone);
                if (old_customer == null)
                {
                    //taoj moi thong tin khacsh hang
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Số điện thoại không tồn tại trong hệ thống bảo hành.",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                    //var customer = new Customer()
                    //{
                    //    Createdate = DateTime.Now,
                    //    Phone = warranti.Phone,
                    //    Chanel = warranti.Chanel,
                    //    Province = warranti.Province,
                    //    District = warranti.District,
                    //    Ward = warranti.Ward,
                    //    Address = warranti.Address,
                    //    Name = warranti.CusName,
                    //};
                    //db.Customers.Add(customer);
                    //db.SaveChanges();
                }
                else
                {
                    //update thong tin khach hang
                    if (!string.IsNullOrEmpty(old_customer.Province))
                    {
                        old_customer.Name = warranti.CusName;
                        old_customer.Province = warranti.Province;
                        old_customer.District = warranti.District;
                        old_customer.Ward = warranti.Ward;
                        old_customer.Address = warranti.Address;

                        db.Entry(old_customer).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                }
                var _warranti = new Warranti();

                _warranti.Phone = warranti.Phone;
                _warranti.PhoneExtra = warranti.Phone2;
                _warranti.Province = warranti.Province;
                _warranti.District = warranti.District;
                _warranti.Ward = warranti.Ward;
                _warranti.Address = warranti.Address;
                _warranti.Note = warranti.Note;
                _warranti.ProductName = warranti.ProductName;
                _warranti.ProductCode = warranti.SerialBrand;
                _warranti.Serial = warranti.SerialHot;
                _warranti.Model = warranti.Model;
                _warranti.Warranti_Cate = warranti.Category;
                _warranti.Createdate = DateTime.Now;
                _warranti.Status = 0;

                if (!string.IsNullOrEmpty(warranti.UserId))
                {
                    var user = db.AspNetUsers.Find(warranti.UserId);
                    _warranti.Createby = user.UserName;
                    //chuyển phiếu cho trạm luôn
                    if (user.AspNetRoles.FirstOrDefault().Id == "Key")
                    {
                        _warranti.Status = 2;
                        _warranti.Station_Warranti = user.UserName;

                    }
                }
                if (!string.IsNullOrEmpty(warranti.Chanel))
                {
                    _warranti.Chanel = warranti.Chanel.ToUpper();
                }
                else
                {
                    _warranti.Chanel = "APP";
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

                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Yêu cầu bảo hành đã được ghi nhận, chúng tôi sẽ liên hệ lại trong vòng 24h. Liên hệ hotline khi cần hỗ trợ thêm. Cảm ơn quý khách",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                var result = new ResultString()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }

        }
        [Route("active")]
        [HttpPost]
        public HttpResponseMessage Active(Active active)
        {
            string active_success = "Sản phẩm {0} đã kích hoạt thành công từ {1} đến ngày {2}. Liên hệ hotline khi cần hỗ trợ thêm. Cảm ơn quý khách";
            if (string.IsNullOrEmpty(active.Customer.Phone))
            {
                var resulter = new ResultString()
                {
                    Status = -1,
                    Message = "Nhập số điện thoại bảo hành",
                    Data = new List<string>()
                };
                return ResponseMessage(resulter);
            }
            if (Utils.Control.getMobileOperator(active.Customer.Phone) == "UNKNOWN")
            {
                var resulter = new ResultString()
                {
                    Status = -1,
                    Message = "Số điện thoại không đúng",
                    Data = new List<string>()
                };
                return ResponseMessage(resulter);
            }
            if (string.IsNullOrEmpty(active.Customer.City) || string.IsNullOrEmpty(active.Customer.District) || string.IsNullOrEmpty(active.Customer.Ward) || string.IsNullOrEmpty(active.Customer.Address))
            {
                var resulter = new ResultString()
                {
                    Status = -1,
                    Message = "Nhập thông tin địa chỉ",
                    Data = new List<string>()
                };
                return ResponseMessage(resulter);
            }
            try
            {
                var product = db.Products.Find(active.IDProduct);
                // lấy code trong tem cào
                var productStamps = db.ProductStamps.FirstOrDefault(a=>a.Code == active.Code);
                var _agency = db.AspNetUsers.FirstOrDefault(a => a.Id == active.UserId);

                if (string.IsNullOrEmpty(active.SerialBrand) || string.IsNullOrEmpty(active.Code))
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Vui lòng nhập Serial máy/ Mã kích hoạt trên tem cào",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                else if (product == null || productStamps == null)
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Serial máy/ Mã cào của sản phẩm không tồn tại",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                else if (product.Status == 1 || productStamps.Status == 1)
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Serial máy/ Mã cào của sản phẩm đã được kích hoạt trước đó",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                else if (_agency != null && _agency.PhoneNumber == active.Customer.Phone)
                {
                    var resulter = new ResultString()
                    {
                        Status = -1,
                        Message = "Số điện thoại không phù hợp để kích hoạt sản phẩm",
                        Data = new List<string>()
                    };
                    return ResponseMessage(resulter);
                }
                else
                {
                    //tao hoac update thong tin khach hang
                    var old_customer = db.Customers.SingleOrDefault(a => a.Phone == active.Customer.Phone);
                    if (old_customer == null)
                    {
                        //taoj moi thong tin khacsh hang
                        old_customer = new Customer()
                        {
                            Createdate = DateTime.Now,
                            Phone = active.Customer.Phone,
                            Chanel = "APP",
                            Name = active.Customer.Name,
                            Province = active.Customer.City,
                            District = active.Customer.District,
                            Ward = active.Customer.Ward,
                            Address = active.Customer.Address
                        };
                        db.Customers.Add(old_customer);
                        db.SaveChanges();
                    }
                    else
                    {
                        //update thong tin khach hang
                        if (!string.IsNullOrEmpty(old_customer.Province))
                        {
                            old_customer.Name = active.Customer.Name;
                            old_customer.Province = active.Customer.City;
                            old_customer.District = active.Customer.District;
                            old_customer.Ward = active.Customer.Ward;
                            old_customer.Address = active.Customer.Address;

                            db.Entry(old_customer).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //update thong tin san pham

                    //dai ly kich hoat
                    if (!string.IsNullOrEmpty(active.UserId))
                    {
                        product.Active_by = db.AspNetUsers.Find(active.UserId).UserName;
                    }
                    ////nếu ngày tạo sản phẩm quá 35 ngày thì phải xử lý khác
                    //var date = DateTime.Now.AddDays(-35);
                    //if (product.Createdate < date)
                    //{
                    //    product.Customer_date = DateTime.Now;
                    //    product.Active_date = product.Createdate.Value.AddDays(35);
                    //    product.End_date = product.Active_date.Value.AddMonths(product.Limited);
                    //}
                    //else
                    //{
                    //    product.Active_date = DateTime.Now;
                    //    product.End_date = DateTime.Now.AddMonths(product.Limited);
                    //}
                    //map thông tin 2 bảng Mã Cào và Sản phẩm
                    productStamps.Status = 1;//đổi stt codestamps sang đã kích hoạt
                    product.Code = productStamps.Code;
                    product.Serial = productStamps.Serial;
                    product.SerialBrand = active.SerialBrand;
                    product.Active_date = DateTime.Now;
                    product.End_date = DateTime.Now.AddMonths(product.Limited);
                    //khach  hang tu kich hoat
                    product.Active_phone = active.Customer.Phone;
                    product.Status = 1;
                    product.Active_chanel = "APP";
                    product.Buydate = DateTime.ParseExact(active.Buydate, "dd/MM/yyyy", null);

                    db.Entry(product).State = EntityState.Modified;
                    //thưởng kích hoạt cho đại lý
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
                                logger.Info(string.Format("APP ACTIVE TOPUP @Phone{0} @Amount{1}", _agency.PhoneNumber, agent_active.Amount));
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
                    //quét thưởng cho khách hàng
                    AddBonus(product.Model, product.Code, old_customer);
                    db.SaveChanges();

                    //show thong tin kich hoat
                    var model = new ProductResult()
                    {
                        Name = product.Name,
                        SerialBrand = product.SerialBrand,
                        Serial = product.Serial,
                        Code = product.Code,
                        Model = product.Model,
                        Trademark = product.Trademark,
                        Activedate = product.Active_date.Value.ToString("dd/MM/yyyy"),
                        Limited = product.Limited.ToString(),
                        Enddate = product.End_date.Value.ToString("dd/MM/yyyy"),
                    };

                    var result = new ProductActiveRes()
                    {
                        Status = 1,
                        Message = string.Format(active_success, product.Code, product.Active_date, product.End_date),
                        Data = new List<ProductResult>() { model }
                    };
                    return ResponseMessage(result);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);

                var resulter = new ResultString()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(resulter);
            }
        }
        void AddBonus(string Model, string ProductCode, Customer customer)
        {
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
                        }
                    }
                    //db.SaveChanges();
                }
            }

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
        [Route("updatedetails")]
        [HttpPost]
        public HttpResponseMessage Update(UpdateDetailReq detailReq)
        {
            try
            {
                string json = JsonConvert.SerializeObject(detailReq);
                logger.Info(json);
                string msg = User.Identity.Name + " Cập nhật trạng thái phiếu. ";
                var warranti = db.Warrantis.Find(detailReq.Id);
                warranti.Warranti_Cate = detailReq.ProductCate;
                warranti.Model = detailReq.Model;
                warranti.ProductCode = detailReq.SerialBrand;
                warranti.Lat = detailReq.Lat;
                warranti.Long = detailReq.Long;
                warranti.Successdate = DateTime.Now;
                warranti.Successnote = detailReq.Note;
                warranti.Sign = detailReq.Sign;
                //hình ảnh nếu có rồi thì thôi
                var _image = db.Warranti_Image.Where(a => a.IdWarranti == warranti.Id);
                if (_image.Count() == 0)
                {
                    if (detailReq.CheckIns.Count() > 0)
                    {
                        foreach (var item in detailReq.CheckIns)
                        {
                            string path = item;
                            if (item.Contains(ConfigControl.DOMAIN))
                            {
                                path = item.Replace(ConfigControl.DOMAIN, "");
                            }
                            var image = new Warranti_Image()
                            {
                                IdWarranti = warranti.Id,
                                Image = path,
                                Type = 1

                            };
                            db.Warranti_Image.Add(image);
                        }
                    }

                    if (detailReq.CheckOuts.Count > 0)
                    {
                        foreach (var item in detailReq.CheckOuts)
                        {
                            string path = item;
                            if (item.Contains(ConfigControl.DOMAIN))
                            {
                                path = item.Replace(ConfigControl.DOMAIN, "");
                            }
                            var image = new Warranti_Image()
                            {
                                IdWarranti = warranti.Id,
                                Image = path,
                                Type = 2

                            };
                            db.Warranti_Image.Add(image);
                        }
                    }
                }
                //phụ tùng nếu có rồi thì thôi
                var _phutung = db.Warranti_Accessary.Where(a => a.IdWarranti == warranti.Id);
                if (_phutung.Count() == 0)
                {
                    if (detailReq.Devices.Count() > 0)
                    {
                        foreach (var item in detailReq.Devices)
                        {
                            if (item.Name == "Tư vấn khách hàng")
                            {
                                msg = msg + "Hoàn thành";
                                warranti.Status = 1;
                                break;
                            }
                            var a_key = db.Accessary_Key.Where(a => a.Id_Key == warranti.Station_Warranti).SingleOrDefault(a => a.Name == item.Name);
                            if (a_key != null && (a_key.CountImport - a_key.CountExport) > 0)
                            {
                                a_key.CountExport = a_key.CountExport + 1;
                                db.Entry(a_key).State = EntityState.Modified;

                                var acc = new Warranti_Accessary()
                                {
                                    IdWarranti = warranti.Id,
                                    Name = a_key.Name,
                                    Price = (int)item.Price,
                                    Quantity = 1,
                                    Amount = warranti.Price
                                };
                                db.Warranti_Accessary.Add(acc);
                                //add log xuat linh kien
                                var logkey = new Acessary_Log_Key()
                                {
                                    Accessary = acc.Name,
                                    Code = warranti.Code,
                                    Count = 1,
                                    Createdate = DateTime.Now,
                                    Id_Akey = a_key.Id,
                                    Type = 2//xuat
                                };
                                db.Acessary_Log_Key.Add(logkey);

                                if (warranti.Warranti_Cate == "Bảo hành" || warranti.Warranti_Cate == "Bảo trì, bảo dưỡng")
                                {
                                    warranti.Price_Accessary = 0;
                                }
                                else
                                {
                                    warranti.Price_Accessary = warranti.Price_Accessary + (int)item.Price;
                                }
                                msg = msg + "Hoàn thành";
                                warranti.Status = 1;
                            }
                            else
                            {
                                msg = msg + "Linh kiện " + a_key.Name + " không còn ở trạm.";
                                warranti.Status = 7;
                                warranti.Price_Accessary = 0;
                                break;
                            }
                        }
                    }
                    else
                    {
                        msg = msg + "Chờ linh kiện";
                        warranti.Status = 7;
                        warranti.Price_Accessary = 0;
                    }
                }
                warranti.KM = detailReq.KM;
                warranti.Price_Move = (int)detailReq.MoveFee;
                warranti.Price = (int)detailReq.Price;
                warranti.Amount = warranti.Price_Accessary + warranti.Price_Move + warranti.Price;
                db.Entry(warranti).State = EntityState.Modified;
                var log = new Warranti_Log()
                {
                    Id_Warranti = warranti.Id,
                    Createdate = DateTime.Now,
                    Description = msg
                };
                db.Warranti_Log.Add(log);
                db.SaveChanges();
                var result = new ResultString()
                {
                    Status = -1,
                    Message = msg,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                var result = new ResultString()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }

        }

        //image
        [Route("uploadimage")]
        [HttpPost]
        public HttpResponseMessage PostUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    string link = "";
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");

                            dict.Add("error", message);
                            return Request.CreateResponse(HttpStatusCode.BadRequest, dict);
                        }
                        else
                        {
                            string strrand = Guid.NewGuid().ToString();


                            var path = HttpContext.Current.Server.MapPath("~/Data/ImageWarr/" + strrand + "_" + postedFile.FileName);

                            postedFile.SaveAs(path);
                            ResizeSettings resizeSetting = new ResizeSettings
                            {
                                MaxWidth = 800,
                                MaxHeight = 800,
                            };
                            ImageBuilder.Current.Build(path, path, resizeSetting);
                            link = "/Data/ImageWarr/" + strrand + "_" + postedFile.FileName;
                        }
                    }

                    var ssresult = new UploadImageRes()
                    {
                        Status = 1,
                        Message = "Tải ảnh lên thành công.",
                        Data = new List<string>() { ConfigControl.DOMAIN + link }
                    };
                    return ResponseMessage(ssresult);
                }
                var result = new UploadImageRes()
                {
                    Status = -1,
                    Message = "Please upload a image",
                    Data = new List<string>() { }
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                var result = new UploadImageRes()
                {
                    Status = -1,
                    Message = ex.Message,
                    Data = new List<string>() { }
                };
                return ResponseMessage(result);
            }
        }

        //order product
        [Route("listproductcate")]
        [HttpGet]
        public HttpResponseMessage Product_Order_Cate(string textSearch = "")
        {
            var model = from a in db.E_ProductCate
                        orderby a.Sort ascending
                        select new ItemsProductCate()
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Descripttion = a.Description,
                            Sort = a.Sort,
                            Thumnails = ConfigControl.DOMAIN + a.Thumnails,
                        };
            var result = new O_Product_Cate()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }

        [Route("listproductorder")]
        [HttpGet]
        public HttpResponseMessage Product_Order_List(long cate = 0, string textSearch = "")
        {
            var model = from a in db.E_Product
                        orderby a.Sort ascending
                        select new ItemProduct()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Name = a.Name,
                            isNew = a.IsNew,
                            Thumnails = ConfigControl.DOMAIN + a.Thumnails,
                            Price = a.Price,
                            Cate = a.Cate
                        };
            if (cate > 0)
            {
                model = model.Where(a => a.Cate == cate);
            }
            var result = new O_Product()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("productorderitem")]
        [HttpGet]
        public HttpResponseMessage Product_Order_Items(long Id)
        {
            var model = from a in db.E_Product
                        where a.Id == Id
                        orderby a.Sort ascending
                        select new Items
                        {
                            Id = a.Id,
                            Images = db.E_Product_Image.Where(i => i.Id_Product == a.Id).Select(i => ConfigControl.DOMAIN + i.Image).ToList(),
                            Name = a.Name,
                            Banner = ConfigControl.DOMAIN + a.Banner,
                            Thumnails = ConfigControl.DOMAIN + a.Thumnails,
                            Code = a.Code,
                            Description = a.Description,
                            ListedPrice = a.ListedPrice,
                            Price = a.Price,
                            Discount = a.Discount,
                            Details = a.Details.Replace("/Data/images/", domain + "Data/images/"),
                            Limited = a.Limited,
                            Link = a.Link,
                            Status = a.Status,
                            Sort = a.Sort,
                            Createdate = a.Createdate,
                            IsNew = a.IsNew,
                            Trademark = a.Trademark,
                            Model = a.Model,
                            Tech = a.Tech,
                            Watt = a.Watt,
                            Size = a.Size,
                            Volt = a.Volt,
                            Madein = a.Madein,
                            Apply = a.Apply,
                            Util = a.Util,
                            Another = a.Another,
                            Extras = db.E_Product_Extra.Where(e => e.IdProduct == a.Id).ToList()
                        };
            var result = new O_Product_Detail()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("listorder")]
        [HttpGet]
        public HttpResponseMessage Order_List(string username, string textSearch = "", int page = 1)
        {
            var model = from a in db.E_Order
                        select new ListOrder()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            UserName = a.Agent_Id,
                            Createdate = a.Createdate,
                            Amount = db.E_OderItems.Where(i => i.Code == a.Code).Sum(s => s.Total),//total là tiền sau chiết khấu
                            LinkDowload = (a.LinkFile != "") ? ConfigControl.DOMAIN + a.LinkFile : ""
                        };
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == username);
                if (user.AspNetRoles.FirstOrDefault().Id == "Admin")
                {

                }
                else
                {
                    model = model.Where(a => a.UserName == username);
                }
            }
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Code == textSearch);
            }
            int pageSize = 10;
            int currentPage = page - 1;
            var result = new O_Order()
            {
                Status = 1,
                Message = "OK",
                Data = model.OrderByDescending(a => a.Createdate).Skip(currentPage * pageSize).Take(pageSize).ToList()
            };
            return ResponseMessage(result);
        }

        [Route("orderitems")]
        [HttpGet]
        public HttpResponseMessage Order_Items(string Code)
        {
            var model = from a in db.E_OderItems
                        where a.Code == Code
                        select new Items_New()
                        {
                            Id = a.Id,
                            ProductId = a.ProductId,
                            ProductThumnail = a.ProductThumnail,
                            Discount = a.Discount,
                            Total = a.Total,
                            Code = a.Code,
                            Product = a.Product,
                            ProductCode = a.ProductCode,
                            Amount = a.Total,
                            Quantity = a.Quantity,
                            ProductUnit = a.ProductUnit,
                            ListedPrice = a.ListedPrice,
                            Price = a.Price
                        };
            var list = model.ToList();
            var result = new Order_Items()
            {
                Status = 1,
                Message = "OK",
                Data = model.ToList()
            };
            return ResponseMessage(result);
        }
        [Route("submitorder")]
        [HttpPost]
        public HttpResponseMessage Order_Submit(O_Order_Req model)
        {
            try
            {
                string message = "Tài khoản không hợp lệ";
                var _agent = db.AspNetUsers.SingleOrDefault(a => a.UserName == model.UserName);
                if (_agent != null && _agent.AspNetRoles.FirstOrDefault().Name == "Đại lý")
                {
                    int Id = 0;
                    var _checkod = db.E_Order.OrderByDescending(a => a.Createdate).FirstOrDefault();
                    if (_checkod != null)
                    {
                        Id = (int)_checkod.Id;
                    }
                    string _code = Utils.Control.CreateCodeOrder(Id);

                    if (model.Items.Count > 0)
                    {
                        int count = 0;
                        int error = 0;
                        int quantity = 0;
                        int amount = 0;
                        foreach (var item in model.Items)
                        {
                            var product = db.E_Product.SingleOrDefault(a => a.Code == item.Code);
                            if (product != null)
                            {

                                int price = product.Price;
                                quantity = quantity + item.Quantity;
                                amount = amount + price * item.Quantity;
                                E_OderItems _item = new E_OderItems()
                                {
                                    Code = _code,
                                    ProductId = product.Id,
                                    Product = product.Name,
                                    ProductCode = product.Code,
                                    Quantity = item.Quantity,
                                    ListedPrice = product.ListedPrice,
                                    Price = price,
                                    Amount = price * item.Quantity,
                                    Total = price * item.Quantity,
                                    Discount = 0,
                                    ProductThumnail = product.Thumnails,
                                    ProductUnit = product.Unit
                                };
                                db.E_OderItems.Add(_item);
                                count++;
                            }
                            else
                            {
                                error++;
                                break;
                            }
                        }
                        E_Order order = new E_Order()
                        {
                            Code = _code,
                            Agent_Id = model.UserName,
                            Status = 0,
                            Createdate = DateTime.Now,
                            Createby = model.UserName,
                            Quantity = quantity,
                            Amount = amount
                        };
                        db.E_Order.Add(order);
                        if (error == 0 && count > 0)
                        {
                            message = "Đã tạo đơn hàng thành công, chờ phản hồi từ Việt Nam Robotic";
                            db.SaveChanges();

                            var log = new E_Order_Log()
                            {
                                Id_Order = order.Id,
                                Createdate = DateTime.Now,
                                User_Id = model.UserName,
                                Description = string.Format("Tạo mới đơn đơn hàng {0}", order.Code)
                            };
                            db.E_Order_Log.Add(log);
                            db.SaveChanges();

                            MailHelper sendMail = new MailHelper();
                            sendMail.ConfigSendMail(order.Agent_Id, order.Code);
                        }

                    }
                    else
                    {
                        message = "Danh sách sản phẩm trong đơn hàng không sẵn sàng";
                    }
                }
                var result = new ResultString()
                {
                    Status = 1,
                    Message = message,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
                var resulter = new ResultString()
                {
                    Status = 1,
                    Message = ex.Message,
                    Data = new List<string>()
                };
                return ResponseMessage(resulter);
            }

        }

        /*
         * get thong tin san pham de xuat kho
         */
        [Route("getexportproduct")]
        [HttpGet]
        public HttpResponseMessage GetProductExport(string serial)
        {
            var product = db.Products.FirstOrDefault(a => a.SerialBrand == serial);
            if (product == null)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Sản phẩm không tồn tại",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            if (!string.IsNullOrEmpty(product.AgentC1))
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = string.Format("Serial máy {0} đã được xuất trước đó", serial),
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            if (product.Status == 1)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Sản phẩm này đã được kích hoạt BH",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            var model = new TK_Product()
            {
                Id = product.Id,
                Name = product.Name,
                Model = product.Model,
                //Serial = product.Code,

            };
            var result_success = new TK_Prodres()
            {
                Status = 1,
                Message = "Sản phẩm chưa được kích hoạt.",
                Data = new List<TK_Product>() { model },
            };
            return ResponseMessage(result_success);
        }
        /*
         * get thong tin san pham de nhap kho
         */
        [Route("checkimportproduct")]
        [HttpGet]
        public HttpResponseMessage CheckProductImport(string serial)
        {
            var product = db.Products.FirstOrDefault(a => a.SerialBrand == serial);
            if (product == null)
            {
                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Sản phẩm hợp lệ",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            else
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Sản phẩm này đã tồn tại trong hệ thống",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
        }
        /*
         * post bill xuat kho san pham cho dai ly
         */
        [Route("exportproduct")]
        [HttpPost]
        public HttpResponseMessage Export_Product(Export_Product model)
        {
            string json = JsonConvert.SerializeObject(model);
            logger.Info(string.Format("[APP API EXPORT ] @MODEL={0}", json));
            // nếu để trống tên đại lý thì lưu xuất kho khách lẻ
            if (!string.IsNullOrEmpty(model.Agent))
            {
                var agent = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Agent);
                if (agent == null)
                {
                    var result = new ResultString()
                    {
                        Status = -1,
                        Message = "Tài khoản đại lý không đúng",
                        Data = new List<string>()
                    };
                    return ResponseMessage(result);
                }
            }
            else
            {
                model.Agent = "Khách lẻ";
            }
            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Createby);
            if (user == null)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Tài khoản không đúng",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            
            if (model.Quantity <= 0)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Số lượng không đúng",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            var now = DateTime.Now.Date;
            var check_export = db.P_Export.Where(a => a.Createdate >= now);
            var export = new P_Export()
            {
                Code = Control.CREATE_CODE_EXPORT(check_export.Count()),
                Createdate = DateTime.Now,
                Createby = model.Createby,
                Status = 0,
                Quantity = model.Quantity,
                Note = model.Note,
                Agent = model.Agent,
                Inventorydate = (model.Inventorydate > 0) ? model.Inventorydate : 30
            };
            db.P_Export.Add(export);
            db.SaveChanges();
            try
            {
                foreach (var item in model.Items)
                {
                    //lưu item cho phieu xuat
                    var export_item = new P_Export_Item()
                    {
                        ExportId = export.Id,
                        //Serial = item.Serial,
                        Model = item.Model,
                        Name = item.Name,
                        SerialBrand = item.SerialBrand,
                    };
                    db.P_Export_Item.Add(export_item);
                    //cap nhat thong tin cho san pham 
                    var product = db.Products.FirstOrDefault(a => a.SerialBrand == item.SerialBrand);
                    product.AgentC1 = model.Agent;
                    product.Exportdate = export.Createdate;
                    product.Wait_day = export.Inventorydate;
                    product.Status = 3;
                    product.Wait_date = DateTime.Now;
                    product.Ag_Name = model.FullName;
                    product.Ag_Phone = model.PhoneNumber;
                    //product.SerialBrand = export_item.SerialBrand;
                    db.Entry(product).State = EntityState.Modified;
                }
                db.SaveChanges();
                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Đã tạo phiếu xuất thành công. Mã phiếu xuất là : " + export.Code,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                //co loi gi thi xoa phieu di cho khoi rac
                db.P_Export.Remove(export);
                db.SaveChanges();
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Có lỗi xảy ra, kiểm tra lại Serial",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
        }
        /*
        * lay ds model
        */
        [Route("getmodelimport")]
        [HttpGet]
        public HttpResponseMessage GetModelImport(string textSearch)
        {
            var model = from a in db.P_Model
                        select a;
            if (!string.IsNullOrEmpty(textSearch))
            {
                model = model.Where(a => a.Model.Contains(textSearch));
            }
            var result_success = new Model_Result_Import()
            {
                Status = 1,
                Message = "Danh sách Model",
                Data = model.ToList(),
            };
            return ResponseMessage(result_success);
        }
        /*
         * post bill nhap kho san pham
         */
        [Route("importproduct")]
        [HttpPost]
        public HttpResponseMessage Import_Product(Import_Product model)
        {
            string json = JsonConvert.SerializeObject(model);
            logger.Info(string.Format("[APP API IMPORT ] @MODEL={0}", json));
            var user = db.AspNetUsers.FirstOrDefault(a => a.UserName == model.Createby);
            if (user == null)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Tài khoản không đúng",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            if (model.Quantity <= 0)
            {
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Số lượng không đúng",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            var now = DateTime.Now.Date;
            var check_import = db.P_Import.Where(a => a.Createdate >= now);
            var check_model = db.P_Model.FirstOrDefault(a => a.Model == model.Model);
            var import = new P_Import()
            {
                Code = Control.CREATE_CODE_IMPORT(check_import.Count()),
                Createdate = DateTime.Now,
                Createby = model.Createby,
                Model = model.Model,
                Name = model.Name,
                Limited = model.Limited,
                Status = 0,
                Note = model.Note,
                Quantity = model.Quantity
            };
            db.P_Import.Add(import);
            db.SaveChanges();
            try
            {
                foreach (var item in model.Items)
                {
                    var check_product = db.Products.FirstOrDefault(a => a.SerialBrand == item.SerialBrand);
                    if (check_product != null)
                    {
                        db.P_Import.Remove(import);
                        db.SaveChanges();
                        var resulter = new ResultString()
                        {
                            Status = -1,
                            Message = "Serial máy" + item.SerialBrand + " sản phẩm đã tồn tại trong hệ thông.",
                            Data = new List<string>()
                        };
                        return ResponseMessage(resulter);
                    }
                    //lưu item cho phieu xuat
                    var import_item = new P_Import_Item()
                    {
                        ImportId = import.Id,
                        //Serial = item.Serial,
                        Model = model.Model,
                        Name = model.Name,
                        SerialBrand = item.SerialBrand,
                    };
                    db.P_Import_Item.Add(import_item);
                    //cap nhat thong tin cho san pham 
                    var product = new Product()
                    {
                        //Serial = item.Serial,
                        SerialBrand = item.SerialBrand,
                        //Code = item.Serial,
                        Limited = model.Limited,
                        Name = model.Name,
                        Model = model.Model,
                        Createdate = import.Createdate,
                        Importdate = import.Createdate,
                        Createby = import.Createby,
                        Trademark = check_model.Trademark,
                        Status = 0,

                    };
                    db.Products.Add(product);
                }
                db.SaveChanges();
                var result = new ResultString()
                {
                    Status = 1,
                    Message = "Đã tạo phiếu nhập thành công. Mã phiếu nhập là : " + import.Code,
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                //co loi gi thi xoa phieu di cho khoi rac
                db.P_Import.Remove(import);
                db.SaveChanges();
                var result = new ResultString()
                {
                    Status = -1,
                    Message = "Có lỗi xảy ra trong danh sách sản phẩm",
                    Data = new List<string>()
                };
                return ResponseMessage(result);
            }
        }
    }
}