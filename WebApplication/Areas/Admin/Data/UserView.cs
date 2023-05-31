using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class UserView : AspNetUser
    {
        public string Role { get; set; }
    }
}