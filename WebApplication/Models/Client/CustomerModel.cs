using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Client
{
    public class CustomerModel : Customer
    {
        public List<ListActive> ListActive { get; set; }
        public List<Warranti> ListWarranti { get; set; }

        public List<B_Payment> ListPayment { get; set; }
    }
    public class ListActive
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Code { get; set; }
        public int Limited { get; set; }
        public DateTime? Active_date { get; set; }
        public DateTime? End_date { get; set; }
        public List<B_Log_Active> Process { get; set; }
    }
}