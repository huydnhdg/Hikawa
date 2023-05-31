using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.FCM
{
    public class SentNotify
    {
        HikawaEntities db = new HikawaEntities();
        public void Sent(string UserName, string Content)
        {
            try
            {
                var user = db.User_Device.OrderByDescending(a=>a.Createdate).FirstOrDefault(a => a.UserName == UserName);
                
                if (user != null)
                {              

                    dynamic data = new
                    {
                        to = user.DeviceId,
                        notification = new
                        {
                            title = "Deborah",
                            body = Content,
                            //link = ""
                        }
                    };
                    var fcmcontrol = new FCMControl();
                    string result = fcmcontrol.SendNotification(data);
                    var model = new FcmResult();
                    if (!string.IsNullOrEmpty(result))
                    {
                        model = JsonConvert.DeserializeObject<FcmResult>(result);
                    }

                    var save = new SentNotifi()
                    {
                        Message = Content,
                        Createdate = DateTime.Now,
                        UserName = UserName,
                        Status = model.failure,
                        Result = result
                    };
                    db.SentNotifis.Add(save);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}