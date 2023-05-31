using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class RunController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        public ActionResult Index()
        {
            var log = db.Agent_Log_Active.Where(a => a.Id > 8);
            var list = log.ToList();
            //foreach (var item in list)
            //{
            //    if (item.UserName == null)
            //    {
            //        continue;
            //    }
            //    var check = db.Agent_Bonus.FirstOrDefault(a => a.UserName == item.UserName);
            //    if (check == null)
            //    {
            //        var create = new Agent_Bonus()
            //        {
            //            UserName = item.UserName,
            //            Active = item.Amount,
            //            Createdate = item.Createdate,
            //            Newdate = item.Createdate
            //        };
            //        db.Agent_Bonus.Add(create);
            //        db.SaveChanges();
            //    }
            //    else
            //    {
            //        check.Active = check.Active + item.Amount;
            //        check.Newdate = item.Createdate;
            //        db.Entry(check).State = EntityState.Modified;
            //        db.SaveChanges();
            //    }
            //}
            return View();
        }
    }
}