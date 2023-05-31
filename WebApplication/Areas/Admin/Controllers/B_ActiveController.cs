using NLog;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class B_ActiveController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        public ActionResult Index(int? page, string phone, string textsearch, string chanel, string status, string start_date, string end_date)
        {
            var active = from a in db.B_Log_Active
                         group a by a.ProductCode into g
                         join b in db.Products on g.Key equals b.Code
                         select new B_G_Active()
                         {
                             Model = g.Select(a => a.Model).FirstOrDefault(),
                             ProductCode = g.Select(a => a.ProductCode).FirstOrDefault(),
                             Status = g.Select(a => a.Status).FirstOrDefault(),
                             Processs = g.Select(a => a.Process).ToList(),
                             Point = g.Select(a => a.Quantity.ToString() + " " + a.Unit).ToList(),
                             Activedate = b.Active_date,
                             Phone = b.Active_phone,
                             CheckActive = g.Select(a => a.CheckActive).FirstOrDefault(),
                             PayContent = "",
                             PayAmount = 0,
                             PayCate = "",
                             isActive = true,
                         };
            var payment = from a in db.B_Payment
                          select new B_G_Active()
                          {
                              Model = "",
                              ProductCode = "",
                              Status = a.Status,
                              Processs = new List<string>() { "DQ" },
                              Point = new List<string>() { a.PointCharge.ToString() },
                              Activedate = null,
                              Phone = a.Phone,
                              CheckActive = a.Createdate,
                              PayContent = a.PayContent,
                              PayAmount = a.PayAmount,
                              PayCate = a.PayCate,
                              isActive = false
                          };

            //if (!string.IsNullOrEmpty(textsearch))
            //{
            //    model = model.Where(a => a.Model.Contains(textsearch)
            //    || a.ProductCode.Contains(textsearch));
            //    ViewBag.textsearch = textsearch;
            //}
            IEnumerable<B_G_Active> data1 = active as IEnumerable<B_G_Active>;
            IEnumerable<B_G_Active> data2 = payment as IEnumerable<B_G_Active>;

            IEnumerable<B_G_Active> data = data1.Concat(data2);
            if (!string.IsNullOrEmpty(phone))
            {
                data = data.Where(a => a.Phone == phone);
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                data = data.Where(a => a.Phone == textsearch
                || a.Model.Contains(textsearch)
                || a.ProductCode.Contains(textsearch)
                || a.PayContent.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                data = data.Where(a => a.isActive.ToString().ToLower() == chanel);
                ViewBag.chanel = chanel;
            }
            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                data = data.Where(a => a.CheckActive >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                data = data.Where(a => a.CheckActive <= s);
                ViewBag.end_date = end_date;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.CheckActive).ToPagedList(pageNumber, pageSize));
        }
    }
}