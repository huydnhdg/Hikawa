using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Product_Export
{
    public class Model_Result_Import: Result
    {
        public List<P_Model> Data { get; set; }
    }
}