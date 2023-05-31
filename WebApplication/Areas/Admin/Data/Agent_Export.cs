using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Data
{
    public class Agent_Export : Agent_Log_Active
    {
        public string Role { get; set; }
        public string FullName { get; set; }
    }
}