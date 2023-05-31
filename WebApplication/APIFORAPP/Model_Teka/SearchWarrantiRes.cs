using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.APIFORAPP.Model;
using WebApplication.Models;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class SearchWarrantiRes : Result
    {
        public List<WarrantiModelReNew> Data { get; set; }
    }
    public class WarrantiModelReNew
    {
        public long Id { get; set; }
        public string SerialBrand { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string CusName { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public DateTime? Createdate { get; set; }
        public string Createby { get; set; }
        public string KeyWarranti { get; set; }
        public string IdKey { get; set; }
        public string IdKTV { get; set; }
        public string KTV { get; set; }
        public string Comment { get; set; }
        public string ProductCate { get; set; }
        public string Model { get; set; }
        public string CodeWarr { get; set; }
        public List<Warranti_Log> LogWarrantis { get; set; }
    }
}