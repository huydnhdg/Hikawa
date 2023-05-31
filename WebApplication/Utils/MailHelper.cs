using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace WebApplication.Utils
{
    public class MailHelper
    {
        public MailHelper() { }
        static Logger logger = LogManager.GetCurrentClassLogger();
        public void ConfigSendMail(string agent, string or_code)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add("ninhtd@bluesea.vn");//mail khach hang
                mail.From = new MailAddress("info@vietnamrobotics.vn");
                mail.Subject = "Hệ thống BHDT thông báo đơn hàng mới";
                mail.Body = MailContent(agent, or_code).ToString();//phan than mail
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("info@vietnamrobotics.vn", "Rbt@37NguyenvawnHuyen");// tài khoản Gmail của bạn
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }
        private StringBuilder MailContent(string agent, string or_code)
        {
            string url = ConfigControl.DOMAIN + "admin/order?textsearch=" + or_code;
            StringBuilder Body = new StringBuilder();
            Body.Append("<p>Hệ thống bảo hành điện tử thông báo: </p>");
            Body.Append("<p>Bạn nhận được 1 đơn hàng mới được tạo bởi " + agent + ". Mã đơn hàng: <b>" + or_code + "</b></p>");
            Body.Append("<a href='" + url + "'>Link xem đơn hàng<a/>");

            string json = JsonConvert.SerializeObject(Body);
            logger.Info(json);
            return Body;
        }
    }
}