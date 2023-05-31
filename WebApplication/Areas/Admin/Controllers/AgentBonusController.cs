using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class AgentBonusController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        //
        static IEnumerable<Agent_Export> listexc = null;
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Agent_Bonus
                        select new AgentBonusModel()
                        {
                            UserName = a.UserName,
                            Used = a.Used,
                            Active = a.Active,
                            Newdate = a.Newdate,
                            Createdate = a.Createdate,
                            Role = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.UserName).AspNetRoles.FirstOrDefault().Id,
                            FullName = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.UserName).FullName,
                        };
            var ex = from a in db.Agent_Log_Active
                     where a.UserName != null
                     select new Agent_Export()
                     {
                         UserName = a.UserName,
                         Model = a.Model,
                         Product = a.Product,
                         Amount = a.Amount,
                         Createdate = a.Createdate,
                         Role = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.UserName).AspNetRoles.FirstOrDefault().Id,
                         FullName = db.AspNetUsers.FirstOrDefault(r => r.UserName == a.UserName).FullName,
                     };
            if (!string.IsNullOrEmpty(textsearch))
            {
                ex = ex.Where(a => a.UserName.Contains(textsearch));
                model = model.Where(a => a.UserName.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                ex = ex.Where(a => a.Role.Contains(chanel));
                model = model.Where(a => a.Role.Contains(chanel));
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Newdate >= s);
                ex = ex.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Newdate <= s);
                ex = ex.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            ViewBag.role = db.AspNetRoles.ToList();
            IEnumerable<AgentBonusModel> data = model as IEnumerable<AgentBonusModel>;
            listexc = ex as IEnumerable<Agent_Export>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Newdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult LogActive(int? page, string agent, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Agent_Log_Active
                        select a;
            if (!string.IsNullOrEmpty(agent))
            {
                model = model.Where(a => a.UserName.Contains(agent));
                ViewBag.agent = agent;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Product.Contains(textsearch));
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
            IEnumerable<Agent_Log_Active> data = model as IEnumerable<Agent_Log_Active>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult LogPayment(int? page, string agent, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.Agent_Log_Payment
                        select a;
            if (!string.IsNullOrEmpty(agent))
            {
                model = model.Where(a => a.UserName.Contains(agent));
                ViewBag.agent = agent;
            }
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.UserName.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Paydate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Paydate <= s);
                ViewBag.end_date = end_date;
            }
            IEnumerable<Agent_Log_Payment> data = model as IEnumerable<Agent_Log_Payment>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Paydate).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Edit(string Id)
        {
            var model = db.Agent_Bonus.Find(Id);
            return PartialView("~/Areas/Admin/Views/AgentBonus/_Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditConfirm([Bind(Include = "")] Agent_Bonus agent_Bonus, int Amount)
        {
            if (ModelState.IsValid)
            {
                //
                var bonus = db.Agent_Bonus.Find(agent_Bonus.UserName);
                int amount = bonus.Active - bonus.Used;//tiền hiện có
                if (amount < Amount)
                {
                    SetAlert("Số tiền không đủ để thanh toán.", "danger");
                    return RedirectToAction("Index");
                }
                int used = agent_Bonus.Used + amount;//tiền đã thanh toán

                bonus.Used = used;
                db.Entry(bonus).State = EntityState.Modified;

                var agent_payment = new Agent_Log_Payment()
                {
                    UserName = agent_Bonus.UserName,
                    Amount = amount,
                    Paydate = DateTime.Now
                };
                db.Agent_Log_Payment.Add(agent_payment);

                db.SaveChanges();
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
            return RedirectToAction("Index");

        }

        public void ELM_LS_KichHoat()
        {
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "STT";
            Sheet.Cells["B1"].Value = "Mã đại lý";
            Sheet.Cells["C1"].Value = "Tên đại lý";
            Sheet.Cells["D1"].Value = "Quyền";
            Sheet.Cells["E1"].Value = "Model";
            Sheet.Cells["F1"].Value = "Mã sản phẩm";
            Sheet.Cells["G1"].Value = "Tiền thưởng";
            Sheet.Cells["H1"].Value = "Ngày kích hoạt";

            int index = 1;
            int row = 2;
            foreach (var item in listexc)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = index++;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.UserName;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.FullName;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Role;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Model;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Product;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Amount;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Createdate != null) ? item.Createdate.Value.ToString("dd/MM/yyyy") : "";

                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}