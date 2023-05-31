using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.FCM
{
    public class FcmResult
    {
        //"{\"multicast_id\":8204702981955248722,\"success\":0,\"failure\":1,\"canonical_ids\":0,\"results\":[{\"error\":\"NotRegistered\"}]}"
        public string success { get; set; }
        public string failure { get; set; }
    }
}