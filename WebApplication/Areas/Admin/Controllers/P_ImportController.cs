using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using WebApplication.Areas.Admin.Data;

namespace WebApplication.Areas.Admin.Controllers
{
    public class P_ImportController : BaseController
    {
        // GET: Admin/P_Import
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.P_Import
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Model.Contains(textsearch)
                || a.Code.Contains(textsearch)
                || a.Name.Contains(textsearch)
                || a.Note.Contains(textsearch)
                || a.Createby.Contains(textsearch)
                || a.Quantity.ToString().Contains(textsearch)
                );
                ViewBag.textsearch = textsearch;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(model.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public ActionResult Views(long Id)
        {
            var model = from a in db.P_Import
                        where a.Id == Id
                        join b in db.P_Import_Item on a.Id equals b.ImportId

                        join c in db.Products on b.Serial equals c.Serial
                        select new P_Import_Details()
                        {
                            Serial = b.Serial,
                            Code = c.Code
                        };
            return PartialView("~/Areas/Admin/Views/P_Import/_Views.cshtml", model.ToList());
        }
    }
}