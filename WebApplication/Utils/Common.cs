using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace WebApplication.Utils
{
    public class Common
    {
        public static string domain = WebConfigurationManager.AppSettings["DOMAIN"];

        public static DateTime lastDayOfMonth()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return lastDayOfMonth;
        }
        public static string getStatusstring(int status)
        {
            switch (status)
            {
                case 0:
                    return "Mới tạo";
                case 1:
                    return "Hoàn thành";
                case 2:
                    return "Chuyển trạm";
                case 3:
                    return "";
                case 4:
                    return "Trạm từ chối";
                case 5:
                    return "Đang xử lý";
                case 6:
                    return "Đem về trạm";
                case 7:
                    return "Đợi linh kiện";
                case 8:
                    return "Chờ phản hồi";
                case 9:
                    return "Hủy bỏ";

                default:
                    //default
                    return null;
            }
        }
    }
}