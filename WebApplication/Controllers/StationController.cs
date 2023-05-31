using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [RoutePrefix("tram-bao-hanh")]
    public class StationController : Controller
    {
        HikawaEntities db = new HikawaEntities();
        [Route]
        public ActionResult Index(string txtSearch, string chanel)
        {
            var model = from a in db.AspNetUsers
                        from b in a.AspNetRoles
                        select new UserView()
                        {
                            Id = a.Id,
                            UserName = a.UserName,
                            PhoneNumber = a.PhoneNumber,
                            Email = a.Email,
                            Createdate = a.Createdate,
                            Createby = a.Createby,
                            Address = a.Address + " " + a.Ward + " " + a.District + " " + a.Province,
                            Role = b.Name,
                            FullName = a.FullName,
                            Province = a.Province
                        };
            if (!string.IsNullOrEmpty(txtSearch))
            {
                model = model.Where(a => a.FullName.Contains(txtSearch) || a.UserName.Contains(txtSearch));
                ViewBag.txtSearch = txtSearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Province.Contains(chanel));
                ViewBag.chanel = chanel;
            }
            ViewBag.role = db.Provinces.ToList();
            return View(model.Where(a => a.Role == "Trạm - Trưởng trạm").ToList());
        }
    }
}