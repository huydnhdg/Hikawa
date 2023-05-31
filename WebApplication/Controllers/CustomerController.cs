using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;
using WebApplication.Models.Client;

namespace WebApplication.Controllers
{
    [RoutePrefix("khach-hang")]
    [Authorize(Roles ="Customer")]
    public class CustomerController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        [Route]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "LoginPhone");
            }
            var model = new CustomerModel();
            var customer = db.Customers.SingleOrDefault(a => a.Phone == User.Identity.Name);
            if (customer != null)
            {
                model.Id = customer.Id;
                model.Phone = customer.Phone;
                model.Name = customer.Name;
                model.Email = customer.Email;
                model.PointActive = customer.PointActive;
                model.PointPayment = customer.PointPayment;
                model.Address = customer.Address;
                model.Ward = customer.Ward;
                model.District = customer.District;
                model.Province = customer.Province;
            }
            else
            {
                model.Phone = User.Identity.Name;
            }
            var active = from a in db.Products
                         where a.Active_phone == User.Identity.Name
                         orderby a.Active_date descending
                         select new ListActive()
                         {
                             Id = a.Id,
                             Model = a.Model,
                             Code = a.Code,
                             Active_date = a.Active_date,
                             End_date = a.End_date,
                             Process = db.B_Log_Active.Where(p => p.ProductCode == a.Code).ToList()
                         };
            var listActive = new List<ListActive>();
            listActive = active.ToList();
            model.ListActive = listActive;

            var warranti = from a in db.Warrantis
                           where a.Phone == User.Identity.Name
                           orderby a.Createdate descending
                           select a;
            model.ListWarranti = warranti.ToList();

            var payment = from a in db.B_Payment
                          where a.Phone == User.Identity.Name
                          orderby a.Createdate descending
                          select a;
            var listPayment = new List<B_Payment>();
            listPayment = payment.ToList();
            model.ListPayment = listPayment;

            return View(model);
        }
        public ActionResult AddWarranti(long Id)
        {
            try
            {
                var active = db.B_Log_Active.Find(Id);
                if (active.Status == false)
                {
                    var product = db.Products.SingleOrDefault(a => a.Code == active.ProductCode);
                    if (product != null)
                    {
                        product.End_date = product.End_date.Value.AddMonths(active.Quantity);
                        db.Entry(product).State = EntityState.Modified;

                        active.Status = true;
                        active.CheckActive = DateTime.Now;
                        db.Entry(active).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("Index");
        }
    }
}