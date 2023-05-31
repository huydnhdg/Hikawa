using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.ModelRequest
{
    public class WarrantiReq
    {
        public string Chanel { get; set; }
        public string CusName { get; set; }
        public string Phone { get; set; }
        public string PhoneExtra { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        //
        public string ProductCode { get; set; }
        public string SerialHot { get; set; }
        public string Model { get; set; }
        public string ProductName { get; set; }
        public string Trademark { get; set; }

        //
        public string Note { get; set; }
        public string Createby { get; set; }
    }
}