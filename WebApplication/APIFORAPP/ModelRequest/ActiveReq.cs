using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.ModelRequest
{
    public class ActiveReq
    {
        public string Phone { get; set; }
        public string CusName { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string Code { get; set; }
        public string Serial { get; set; }
        public string SerialBrand { get; set; }
        public string Buydate { get; set; }
        public string Chanel { get; set; }
        public string Active_by { get; set; }
        public string Email { get; set; }
        public long Id { get; set; }
    }
}