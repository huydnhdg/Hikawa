using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [RoutePrefix("huong-dan")]
    public class HelpController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        [Route]
        public ActionResult Index()
        {
            var model = db.Articles.Where(a => a.Link == "huong-dan-su-dung").SingleOrDefault();
            return View(model);
        }
    }
}