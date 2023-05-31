using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.APIFORAPP.Model_Teka
{
    public class UpdateDetailReq
    {
        public long Id { get; set; }//id phieu bh
        public int? Status { get; set; }
        public string SerialBrand { get; set; }
        public string Phone { get; set; }
        public string CusName { get; set; }
        public string Address { get; set; }
        public string Details { get; set; }
        public DateTime? Createdate { get; set; }
        public string KTV { get; set; }
        public string PhoneKTV { get; set; }
        public string KeyWarranti { get; set; }
        public string ProductCate { get; set; }
        public string Model { get; set; }
        public string Note { get; set; }//loi cu the
        public string SerialHot { get; set; }//seri dan nong
        public string SerialCool { get; set; }//seri dan lanh
        public List<string> CheckIns { get; set; }//list anh check in Image1
        public List<string> CheckOuts { get; set; }//list anh check out Image1
        public List<PhuTung> Devices { get; set; }//list phu tung 
        public int FeeSuggest { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public string KM { get; set; }//khoang cach
        public double? MoveFee { get; set; }//phi di chuyen
        public double? Price { get; set; }//phi bh
        public double? Amount { get; set; }//togn chi phi
        public string Sign { get; set; }

    }
    public class PhuTung
    {
        public string ID { get; set; }
        public Nullable<long> IDWarranti { get; set; }
        public string IDPhutung { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Name { get; set; }
    }
}