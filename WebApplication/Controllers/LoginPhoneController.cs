using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [RoutePrefix("dang-nhap")]
    public class LoginPhoneController : Controller
    {
        [Route]
        public ActionResult Index(string Phone="")
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    return Redirect("/Admin/Product/Index");
                }
            }
            ViewBag.phone = Phone;
            return View();
        }
        [HttpPost]
        public ActionResult Register(string phone)
        {
            var model = new LoginViewModel()
            {
                UserName = phone
            };
            return PartialView("~/Views/LoginPhone/_Register.cshtml", model);
        }
        [HttpPost]
        public ActionResult Login(string phone)
        {
            var model = new LoginViewModel()
            {
                UserName = phone
            };
            return PartialView("~/Views/LoginPhone/_Login.cshtml", model);
        }
    }
}