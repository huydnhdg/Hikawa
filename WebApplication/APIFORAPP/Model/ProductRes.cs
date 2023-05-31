using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model
{
    public class ProductRes:Result
    {
        public List<Product> Data { get; set; }
    }
    public class ProductStampsRes : Result
    {
        public List<ProductStamp> Data { get; set; }
    }
    public class CodeStampsRes : Result
    {
        public ProductStamp Data { get; set; }
    }
}