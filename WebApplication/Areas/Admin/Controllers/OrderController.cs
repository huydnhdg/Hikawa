using Aspose.Pdf;
using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.FCM;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Areas.Admin.Controllers
{
    public class OrderController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            string UserName = User.Identity.Name;
            var model = from a in db.E_Order
                        where a.Status != -1// đang trong giỏ hàng
                        join b in db.AspNetUsers on a.Agent_Id equals b.UserName
                        select new E_Order_Model()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Agent_Id = a.Agent_Id,
                            AgentName = b.FullName,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Checkdate = a.Checkdate,
                            Checkby = a.Checkby,
                            Status = a.Status,
                            CountProduct = db.E_OderItems.Where(i => i.Code == a.Code).Count(),
                            SumAmount = db.E_OderItems.Where(i => i.Code == a.Code).Count() > 0 ? db.E_OderItems.Where(i => i.Code == a.Code).Sum(i => i.Total) : 0
                        };

            if (User.IsInRole("Đại lý"))
            {
                model = model.Where(a => a.Createby == UserName);
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.StartsWith(textsearch)
                || a.Agent_Id.Contains(textsearch)
                || a.Createby.Contains(textsearch)
                || a.AgentName.Contains(textsearch));
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
            IEnumerable<E_Order_Model> data = model as IEnumerable<E_Order_Model>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Views(long Id)
        {
            var model = from a in db.E_Order
                        where a.Id == Id
                        join b in db.AspNetUsers on a.Agent_Id equals b.UserName
                        select new E_Order_Details()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Agent_Id = a.Agent_Id,
                            AgentName = b.FullName,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Checkdate = a.Checkdate,
                            Checkby = a.Checkby,
                            Status = a.Status,
                            CountProduct = db.E_OderItems.Where(i => i.Code == a.Code).Count(),
                            SumAmount = db.E_OderItems.Where(i => i.Code == a.Code).Count() > 0 ? db.E_OderItems.Where(i => i.Code == a.Code).Sum(i => i.Total) : 0,
                            Items = db.E_OderItems.Where(i => i.Code == a.Code).ToList()
                        };
            return PartialView("~/Areas/Admin/Views/Order/_Views.cshtml", model.FirstOrDefault());
        }
        public ActionResult ViewPDF(long Id)
        {


            var model = from a in db.E_Order
                        where a.Id == Id
                        join b in db.AspNetUsers on a.Agent_Id equals b.UserName
                        select new E_Order_Details()
                        {
                            Id = a.Id,
                            Code = a.Code,
                            Agent_Id = a.Agent_Id,
                            AgentName = b.FullName,
                            Address = b.Address,
                            Phone = b.PhoneNumber,
                            Email = b.Email,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Checkdate = a.Checkdate,
                            Checkby = a.Checkby,
                            Status = a.Status,
                            Quantity = a.Quantity,
                            Amount = a.Amount,
                            Total = a.Total,
                            CountProduct = db.E_OderItems.Where(i => i.Code == a.Code).Count(),
                            SumAmount = db.E_OderItems.Where(i => i.Code == a.Code).Count() > 0 ? db.E_OderItems.Where(i => i.Code == a.Code).Sum(i => i.Total) : 0,
                            Items = db.E_OderItems.Where(i => i.Code == a.Code).ToList()
                        };
            return View(model.FirstOrDefault());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm([Bind(Include = "")] E_Order_Details details)
        {
            try
            {
                var order = db.E_Order.Find(details.Id);
                if (order.Status != 1)
                {
                    int total = 0;
                    int qty = 0;
                    int discount = 0;
                    int amount = 0;
                    //neu chinh sua chiet khau vs so luong
                    if (details.Items.Count() > 0)
                    {
                        foreach (var item in details.Items)
                        {
                            var product = db.E_OderItems.Find(item.Id);
                            product.Quantity = item.Quantity;
                            product.Amount = item.Amount;
                            product.Discount = item.Discount;
                            product.Total = item.Total;
                            db.Entry(product).State = EntityState.Modified;

                            amount = amount + product.Amount;
                            total = total + product.Total;
                            qty = qty + product.Quantity;
                            discount = discount + (amount - total);//tổng tiền hàng - tiền phải trả
                        }
                        //db.SaveChanges();
                    }
                    order.Status = 1;
                    order.Checkby = User.Identity.Name;
                    order.Checkdate = DateTime.Now;
                    order.Quantity = qty;
                    order.Amount = amount;
                    order.Discount = discount;
                    order.Total = total;
                    //lưu thông tin duyệt đơn hàng

                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    var sent = new SentNotify();
                    sent.Sent(order.Agent_Id, string.Format("Đơn hàng {0} đã được duyệt thành công", order.Code));

                    //tao file pdf cập nhật lại 
                    ConvertPDF convert = new ConvertPDF();
                    string linkPDF = convert.ConvertFromUrl(order.Id, "_" + order.Code);
                    order.LinkFile = linkPDF;
                    db.Entry(order).State = EntityState.Modified;

                    var log = new E_Order_Log()
                    {
                        Id_Order = order.Id,
                        Createdate = DateTime.Now,
                        User_Id=order.Agent_Id,
                        Description = string.Format("Đơn hàng {0} đã được duyệt thành công", order.Code)
                    };
                    db.E_Order_Log.Add(log);
                    db.SaveChanges();
                }

                SetAlert("Đã duyệt đơn hàng thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.E_Order.Find(Id);
                db.E_Order.Remove(model);
                var item = db.E_OderItems.Where(a => a.Code == model.Code);
                if (item.Count() > 0)
                {
                    db.E_OderItems.RemoveRange(item);
                }
                var log = db.E_Order_Log.Where(a => a.Id_Order == model.Id);
                if (log.Count() > 0)
                {
                    db.E_Order_Log.RemoveRange(log);
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
        public ActionResult Cancel(long Id)
        {
            try
            {
                var order = db.E_Order.Find(Id);
                order.Status = 3;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                var sent = new SentNotify();
                sent.Sent(order.Agent_Id, string.Format("Đơn hàng {0} đã bị huỷ bỏ", order.Code));

                var log = new E_Order_Log()
                {
                    Id_Order = order.Id,
                    Createdate = DateTime.Now,
                    User_Id = order.Agent_Id,
                    Description = string.Format("Đơn hàng {0} đã bị huỷ bỏ", order.Code)
                };
                db.E_Order_Log.Add(log);
                db.SaveChanges();

                SetAlert("Huỷ đơn hàng thành công.", "success");
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
            var log = db.E_Order_Log.Where(a => a.Id_Order == Id);
            ViewBag.Code = db.E_Order.Find(Id).Code;
            return PartialView("~/Areas/Admin/Views/Order/_Viewlog.cshtml", log.ToList());
        }
        public ActionResult ChangeStatus(long Id)
        {
            var order = db.E_Order.Find(Id);
            if (order.Status != 1)
            {
                int total = db.E_OderItems.Where(a => a.Code == order.Code).Sum(a => a.Total);
                int qty = db.E_OderItems.Where(a => a.Code == order.Code).Sum(a => a.Quantity);
                int discount = 0;
                int amount = db.E_OderItems.Where(a => a.Code == order.Code).Sum(a => a.Amount);

                order.Status = 1;
                order.Checkby = User.Identity.Name;
                order.Checkdate = DateTime.Now;

                order.Quantity = qty;
                order.Amount = amount;
                order.Discount = discount;
                order.Total = total;

                //tao file pdf
                ConvertPDF convert = new ConvertPDF();
                string linkPDF = convert.ConvertFromUrl(order.Id, "_" + order.Code);
                order.LinkFile = linkPDF;

                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                var sent = new SentNotify();
                sent.Sent(order.Agent_Id, string.Format("Đơn hàng {0} đã được duyệt thành công", order.Code));

                var log = new E_Order_Log()
                {
                    Id_Order = order.Id,
                    Createdate = DateTime.Now,
                    User_Id = order.Agent_Id,
                    Description = string.Format("Đơn hàng {0} đã được duyệt thành công", order.Code)
                };
                db.E_Order_Log.Add(log);
                db.SaveChanges();

                SetAlert("Đã duyệt nhanh đơn hàng thành công.", "success");
            }
            else
            {
                SetAlert("Duyệt nhanh đơn hàng không thành công.", "danger");
            }
            return RedirectToAction("Index");
        }
    }
}