using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class ReportController : BaseController
    {
        
        public ActionResult Index()
        {
            var model = new HomeView();

            var warranti = from a in db.Warrantis
                           select a;
            var user = from a in db.AspNetUsers
                       select a;
            var active = from a in db.Products
                         select a;

            if (User.IsInRole("Đại lý"))
            {
                warranti = warranti.Where(a => a.Createby == User.Identity.Name);
                active = active.Where(a => a.Active_by == User.Identity.Name);
            }
            else if (User.IsInRole("Trạm - Trưởng trạm"))
            {
                warranti = warranti.Where(a => a.Station_Warranti == User.Identity.Name);
                active = active.Where(a => a.Active_by == User.Identity.Name);
                model.taikhoan = user.Where(a => a.Createby == User.Identity.Name).Count();
            }
            else if (User.IsInRole("Trạm - Nhân viên kỹ thuật"))
            {
                warranti = warranti.Where(a => a.KTV_Warranti == User.Identity.Name);
                active = active.Where(a => a.Active_by == User.Identity.Name);
            }
            else
            {
                model.taikhoan = user.Where(a => a.AspNetRoles.FirstOrDefault().Id == "Agent").Count();
            }

            model.sanpham = active.Count();
            model.baohanh = warranti.Count();
            model.kichhoat = active.Where(a => a.Status == 1).Count();


            model.ac_APP = active.Where(a => a.Status == 1 && a.Active_chanel == "APP").Count();
            model.ac_SMS = active.Where(a => a.Status == 1 && a.Active_chanel == "SMS").Count();
            model.ac_WEB = active.Where(a => a.Status == 1 && a.Active_chanel == "WEB").Count();

            model.ac_APP_per = ((float)model.ac_APP / (float)model.kichhoat) * 100;
            model.ac_SMS_per = ((float)model.ac_SMS / (float)model.kichhoat) * 100;
            model.ac_WEB_per = ((float)model.ac_WEB / (float)model.kichhoat) * 100;

            model.wa_APP = warranti.Where(a => a.Chanel == "APP").Count();
            model.wa_SMS = warranti.Where(a => a.Chanel == "SMS").Count();
            model.wa_WEB = warranti.Where(a => a.Chanel == "WEB").Count();
            model.wa_CMS = warranti.Where(a => a.Chanel == "CMS").Count();


            model.wa_APP_per = ((float)model.wa_APP / (float)model.baohanh) * 100;
            model.wa_SMS_per = ((float)model.wa_SMS / (float)model.baohanh) * 100;
            model.wa_WEB_per = ((float)model.wa_WEB / (float)model.baohanh) * 100;
            model.wa_CMS_per = ((float)model.wa_CMS / (float)model.baohanh) * 100;

            var key = from a in db.AspNetUsers
                      from b in a.AspNetRoles
                      where b.Id == "Key"
                      select new KeyReport()
                      {
                          Name = a.UserName + " - " + a.FullName,
                          Warranti_Create = db.Warrantis.Where(w => w.Createby == a.UserName).Count(),
                          Warranti_Receive = db.Warrantis.Where(w => w.Station_Warranti == a.UserName && w.Createby == a.UserName).Count(),//danh sach duoc phan cong tru di tu tao
                          Amount = db.Warrantis.Where(w => w.Station_Warranti == a.UserName).Sum(w => w.Amount),
                      };
            model.keyreport = key.ToList();

            return View(model);
        }
    }
}