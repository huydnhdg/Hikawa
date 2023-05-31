using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WebApplication.FCM
{
    public class FCMControl
    {
        public string SendNotification(object data)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);

            return SendNotification(byteArray);
        }
        public string SendNotification(Byte[] byteArray)
        {
            try
            {

                string SERVER_API_KEY = "AAAArrD8Bus:APA91bG9VZb01jbNOidWvD8IZ5LZoF8pLZzPd4DjII0Pfdua9pXXR-PxqnrS1s6Dt3kLVtjz0i7brGzDZWC6ASOAmBWVQxB0DdzGwDMknZCdtn5WcUAfqsWN5wG7xiFZ245hrKYnCgmu";
                string SENDER_ID = "750293616363";

                WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse tResponse = tRequest.GetResponse();

                dataStream = tResponse.GetResponseStream();

                StreamReader tReader = new StreamReader(dataStream);

                String sResponseFromServer = tReader.ReadToEnd();

                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return sResponseFromServer;
            }
            catch (Exception)
            {                
                throw;
            }
        }
        
    }
}