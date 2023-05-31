using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Teka.Model_Request
{
    public class TK_Warranti
    {
        public string UserId { get; set; }

        public long IDProduct { get; set; }
        public string ProductName { get; set; }

        public string Model { get; set; }
        public string SerialHot { get; set; }
        public string SerialBrand { get; set; }

        public long IdCustomer { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string CusName { get; set; }
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string Province { get; set; }

        public string Note { get; set; }

        public string ProductCate { get; set; }//phan loai san pham
        public string Agent { get; set; }//dai ly
        public string Extra { get; set; }//ghi chu
        public string Chanel { get; set; }//kenh
        public string Category { get; set; }//phan loai
    }
}