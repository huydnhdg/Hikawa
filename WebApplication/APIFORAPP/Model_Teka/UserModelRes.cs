using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class UserModelRes : Result
    {
        public List<UserModel> Data { get; set; }
    }
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? Createdate { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Ward { get; set; }
        public string District { get; set; }
        public string BankAccount { get; set; }
        public string BankAccountHolder { get; set; }
        public string BankName { get; set; }
        public string Nguoidaidien { get; set; }
        public string AgentName { get; set; }
        public string Taxcode { get; set; }
        public string Role { get; set; }
    }
}