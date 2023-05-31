using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class LogWarrRes : Result
    {
        public List<Warranti_Log> Data { get; set; }
    }
}