using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class ConfigBonusController : BaseController
    {
        //
        public ActionResult Index()
        {
            var model = from a in db.Config_Bonus
                        select a;
            return View(model);
        }
        [HttpPost]
        public JsonResult ChangeStatus(long id)
        {
            var user = db.Config_Bonus.Find(id);
            user.Status = !user.Status;
            db.Entry(user).State = EntityState.Modified;

            //db.SaveChanges();
            var log = new Log_Action()
            {
                Createby = User.Identity.Name,
                Createdate = DateTime.Now,
                Type = "config bonus",
                Status = user.Status
            };
            db.Log_Action.Add(log);

            db.SaveChanges();
            return Json(new
            {
                status = user.Status
            });
        }
    }
}