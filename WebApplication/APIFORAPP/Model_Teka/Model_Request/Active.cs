using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Teka.Model_Request
{
    public class Active
    {
        public string UserId { get; set; }
        public long IDProduct { get; set; }
        public string Buydate { get; set; }
        public string BuyAdr { get; set; }
        public string Code { get; set; }
        public string SerialBrand { get; set; }
        public APICustomer Customer { get; set; }
    }
    public class APICustomer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
    }
}