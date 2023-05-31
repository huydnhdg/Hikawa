using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [RoutePrefix("bao-hanh")]
    public class WarrantiController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        [Route]
        public ActionResult Index()
        {
            return View();
        }
        //get list Serial đã kích hoạt theo Sđt
        [HttpPost]
        public JsonResult GetSerialBrand(string text, string phone)
        {
            var product = (from a in db.Products
                           where a.SerialBrand.Contains(text) && a.Active_phone == phone
                           select a);
            return Json(product, JsonRequestBehavior.AllowGet);
        }
    }
}