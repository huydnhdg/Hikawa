using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class DevicePriceRes : Result
    {
        public List<PhutungPriceModel> Data { get; set; }
    }
    public class PhutungPriceModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Cate { get; set; }
        public string Phanloai { get; set; }
        public int? Price { get; set; }
    }
}