using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Utils
{
    public class SendBrandNameVMG
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static string TEMP_MT = "ELMICH: Ma OTP tren web baohanh.elmich.vn cua ban la {0}. Ma nay se het han trong 3 phut. Vui long KHONG chia se ma OTP duoi bat ky hinh thuc nao.";
        static string URL = "http://api.brandsms.vn/api/SMSBrandname";
        static string TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c24iOiJlbG1pY2giLCJzaWQiOiIwZjQ2MjY3Ny0wODRhLTRjNjYtOTYxYy05MGJlNzk2MGM5ODgiLCJvYnQiOiIiLCJvYmoiOiIiLCJuYmYiOjE2MzIxMTQ1NDQsImV4cCI6MTYzMjExODE0NCwiaWF0IjoxNjMyMTE0NTQ0fQ.uHeK72kYm7Vpn3mlL7MTEgnjX-jkHagkDqsftk0QEvU";
        public static string SendSMS(string phone, string OTP)
        {
            var body = new BodyData()
            {
                to = phone,
                message = string.Format(TEMP_MT, OTP),
                from = "ELMICH",
                type = 1,
                scheduled = ""
            };
            string json = JsonConvert.SerializeObject(body);
            logger.Info(json);
            var client = new RestClient(URL + "/SendSMS");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("token", TOKEN);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            string result = response.Content;
            Console.WriteLine(result);
            logger.Info(result);

            var userObj = JObject.Parse(result);
            var status = Convert.ToString(userObj["errorCode"]);
            return status;
        }
        public class BodyData
        {
            public string to { get; set; }
            public string message { get; set; }
            public string from { get; set; }
            public int type { get; set; }
            public string scheduled { get; set; }
        }
    }
}