using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Models.Client;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    [RoutePrefix("dat-hang")]
    public class OrderProductController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        [Route]
        public ActionResult Index()
        {
            var model = db.E_ProductCate.OrderBy(a => a.Sort).OrderBy(a => a.Name);
            return View(model.ToList());
        }

        [Route("{category}")]
        public ActionResult ListProduct(string category)
        {

            var result = new List<E_Product>();
            var cate = db.E_ProductCate.FirstOrDefault(a => a.Link == category);
            if (cate != null)
            {
                var model = db.E_Product.Where(a => a.Cate == cate.Id).OrderBy(a => a.Sort).OrderByDescending(a => a.Createdate);
                result = model.ToList();
                ViewBag.title = cate.Name;
                ViewBag.alt = cate.Link;
            }

            return View(result);
        }

        [Route("{category}/{rewrite}")]
        public ActionResult Product(string category, string rewrite)
        {
            var cate = db.E_ProductCate.FirstOrDefault(a => a.Link == category);
            var list = db.E_Product.Where(a => a.Cate == cate.Id).Where(a => a.Link != rewrite).OrderBy(a => a.Sort).OrderByDescending(a => a.Createdate);

            var query = from a in db.E_Product
                        join b in db.E_ProductCate on a.Cate equals b.Id into temp
                        from t in temp.DefaultIfEmpty()
                        select new E_Product_View()
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Link = a.Link,
                            ListedPrice = a.ListedPrice,
                            Price = a.Price,
                            Limited = a.Limited,
                            Description = a.Description,
                            LongDescription = a.LongDescription,
                            CateName = t.Name,
                            CateLink = t.Link,
                            Details = a.Details,
                            Trademark = a.Trademark
                        };
            var model = query.FirstOrDefault(a => a.Link == rewrite);
            var getExtra = db.E_Product_Extra.Where(a => a.IdProduct == model.Id);
            if (getExtra != null)
            {
                model.Extra = getExtra.ToList();
            }
            var getImage = db.E_Product_Image.Where(a => a.Id_Product == model.Id);
            if (getImage != null)
            {
                model.ListImage = getImage.ToList();
            }
            model.ListProduct = list.ToList();
            ViewBag.title = model.Name;

            return View(model);
        }
        [Route("gio-hang")]
        public ActionResult Cart(int countOrder = 0)
        {
            if (countOrder > 0)
            {
                ViewBag.countOrder = countOrder;
                return View();
            }
            var order = from a in db.E_Order
                        where a.Agent_Id == User.Identity.Name && a.Status == -1
                        select new E_Order_View()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Agent_Id = a.Agent_Id,
                            Createdate = a.Createdate,
                            Amount = a.Amount,
                            Quantity = a.Quantity,
                            Items = db.E_OderItems.Where(o => o.Code == a.Code).ToList()
                        };
            if (order != null)
            {
                return View(order.FirstOrDefault());
            }
            return View();
        }
        [Route("thanh-toan")]
        public ActionResult Payment(long? Id)
        {
            var order = db.E_Order.Find(Id);
            order.Status = 0;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            string url = "/dat-hang/gio-hang?countOrder=" + order.Quantity;


            var log = new E_Order_Log()
            {
                Id_Order = order.Id,
                Createdate = DateTime.Now,
                User_Id = User.Identity.Name,
                Description = string.Format("Tạo mới đơn đơn hàng {0}", order.Code)
            };
            db.E_Order_Log.Add(log);
            db.SaveChanges();

            MailHelper sendMail = new MailHelper();
            sendMail.ConfigSendMail(order.Agent_Id, order.Code);
            return Redirect(url);
        }

        public ActionResult Remove(long? id)
        {
            string url = "/dat-hang/gio-hang";
            var item = db.E_OderItems.Find(id);
            var order = db.E_Order.FirstOrDefault(a => a.Code == item.Code);

            if (order == null)
            {
                return Redirect(url);
            }

            order.Quantity = order.Quantity - item.Quantity;
            order.Amount = order.Amount - item.Amount;
            db.Entry(order).State = EntityState.Modified;
            db.E_OderItems.Remove(item);
            db.SaveChanges();

            //không còn sản phẩm thì xoá luôn phiếu
            if (order.Amount == 0)
            {
                db.E_Order.Remove(order);
                db.SaveChanges();
            }


            return Redirect(url);
        }

        [HttpPost]
        public JsonResult AddCart(long ProductID, int Quantity)
        {
            var _agent = db.AspNetUsers.SingleOrDefault(a => a.UserName == User.Identity.Name);
            if (_agent != null && _agent.AspNetRoles.FirstOrDefault().Name == "Đại lý")
            {
                int IdCode = 0;
                string orderCode = "";
                int qty = 0;

                var _checkod = db.E_Order.OrderByDescending(a => a.Createdate).FirstOrDefault();
                var product = db.E_Product.Find(ProductID);
                int price = product.Price != 0 ? product.Price : product.ListedPrice;

                if (_checkod != null)
                {
                    IdCode = (int)_checkod.Id;
                }
                var _order = db.E_Order.FirstOrDefault(a => a.Agent_Id == User.Identity.Name && a.Status == -1);
                //cập nhật thêm sản phẩm
                if (_order != null)
                {
                    _order.Quantity = _order.Quantity + Quantity;
                    _order.Amount = _order.Amount + price * Quantity;
                    _order.Createdate = DateTime.Now;
                    db.Entry(_order).State = EntityState.Modified;
                    orderCode = _order.Code;
                    qty = _order.Quantity;
                }
                else
                {
                    //thêm mới đơn hàng
                    string _code = Utils.Control.CreateCodeOrder(IdCode);
                    E_Order order = new E_Order()
                    {
                        Code = _code,
                        Agent_Id = User.Identity.Name,
                        Status = -1,
                        Createdate = DateTime.Now,
                        Createby = User.Identity.Name,
                        Quantity = Quantity,
                        Amount = price * Quantity,
                        Total = price * Quantity,
                    };
                    db.E_Order.Add(order);
                    orderCode = order.Code;
                    qty = order.Quantity;
                }

                var checkItems = db.E_OderItems.Where(a => a.Code == orderCode).FirstOrDefault(a => a.ProductCode == product.Code);
                if (checkItems != null)
                {
                    //thêm số lượng
                    checkItems.Quantity = checkItems.Quantity + Quantity;
                    checkItems.Amount = checkItems.Price * checkItems.Quantity;
                    db.Entry(checkItems).State = EntityState.Modified;
                }
                else
                {
                    // thêm mới sản phẩm
                    E_OderItems _item = new E_OderItems()
                    {
                        Code = orderCode,
                        ProductId = product.Id,
                        Product = product.Name,
                        ProductCode = product.Code,
                        ProductThumnail = product.Thumnails,
                        ProductUnit = product.Unit,

                        Quantity = Quantity,
                        ListedPrice = product.ListedPrice,
                        Price = price,
                        Amount = price * Quantity
                    };
                    db.E_OderItems.Add(_item);
                }
                db.SaveChanges();
                return Json(qty, JsonRequestBehavior.AllowGet);
            }

            return Json(-1, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeQuantity(long ProductID, int Quantity)
        {

            var product = db.E_OderItems.Find(ProductID);
            int price = product.Price != 0 ? product.Price : product.ListedPrice;
            product.Amount = price * Quantity;
            product.Total = price * Quantity;
            product.Quantity = Quantity;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            int or_amount = db.E_OderItems.Where(i => i.Code == product.Code).Sum(i => i.Total);
            int or_qty = db.E_OderItems.Where(i => i.Code == product.Code).Sum(i => i.Quantity);

            var order = db.E_Order.FirstOrDefault(a => a.Code == product.Code);
            order.Amount = or_amount;
            order.Quantity = or_qty;
            db.Entry(order).State = EntityState.Modified;

            db.SaveChanges();

            return Json(new
            {
                status = true,
                itemamount = product.Amount.ToString("N0"),
                orderamount = order.Amount.ToString("N0"),
            });
        }
    }
}