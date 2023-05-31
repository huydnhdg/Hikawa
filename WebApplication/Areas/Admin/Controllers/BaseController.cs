using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin - Quản trị toàn hệ thống, Đại lý, Trạm - Nhân viên kỹ thuật, Trạm - Trưởng trạm, Nhân viên, Thủ kho")]
    public abstract partial class BaseController : Controller
    {
        public HikawaEntities db = new HikawaEntities();
        public BaseController()
        {

        }
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;

            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";
                TempData["Alert"] = "Success!";
            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
                TempData["Alert"] = "Warning!";
            }
            else if (type == "danger")
            {
                TempData["AlertType"] = "alert-danger";
                TempData["Alert"] = "Danger!";
            }
            else if (type == "info")
            {
                TempData["AlertType"] = "alert-info";
                TempData["Alert"] = "Info!";
            }
            else if (type == "primary")
            {
                TempData["AlertType"] = "alert-primary";
                TempData["Alert"] = "Primary!";
            }
            else
            {
                TempData["AlertType"] = "alert-default";
                TempData["Alert"] = "Nothing!";
            }

        }
    }
}