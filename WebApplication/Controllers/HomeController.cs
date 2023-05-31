using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        public ActionResult Index()
        {
            var banner = db.Articles.Where(a => a.Cate == "banner - web");
            return View(banner);
        }

        public PartialViewResult _LoadCart()
        {
            int count = 0;
            var order = db.E_Order.FirstOrDefault(a => a.Agent_Id == User.Identity.Name && a.Status == -1);
            if (order != null)
            {
                count = order.Quantity;
            }
            return PartialView(count);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}