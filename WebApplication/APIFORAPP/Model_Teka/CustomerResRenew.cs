using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class CustomerResRenew : Result
    {
        public List<CustomerModel> Data { get; set; }
    }
    public class CustomerModel
    {
        public long? Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public List<Product> ListProduct { get; set; }
    }
}