using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;
using WebApplication.Utils;

namespace WebApplication.Controllers
{
    public class ViewPDFController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        // GET: ViewPDF
        public ActionResult Index(long Id)
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
    }
}