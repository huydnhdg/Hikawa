using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace WebApplication.Utils
{
    public class TOPUP
    {
        public static string TopuptoUserId(string User_id, string Total)
        {
            try
            {
                string data = "";
                string User_ID = User_id;//sdt topup
                string provider = Utils.Control.getMobileOperator(User_id);//lay thong tin mang dien thoai
                string Amount = Total;// so tien topup
                string mode = "1";//cong tien vao tai khoan 0 lay ma the cao 
                string merchant_code = "Hikawa";
                string merchant_key = "ELMICH@397*#^";//key giai ma chuoi ket qua

                String RenewString = String.Format(RenewXMLTemplate, User_ID, Amount, mode, provider, merchant_code);
                String response = CallWebService("http://sms.bluesea.vn:8077/Card/TopUpCard", "http://tempuri.org/TopUpCard", RenewString);
                StringReader str = new StringReader(response);
                XmlReader reader = XmlReader.Create(str);
                reader.ReadToDescendant("ns2:TopUpCardResponse");
                XmlReader result = reader.ReadSubtree();
                if (result.ReadToFollowing("return"))
                {
                    string s = result.ReadInnerXml();
                    string result_allow = Decrypt(merchant_key, s);
                    string[] words = result_allow.Split('|');
                    data = words[0];
                }
                return data;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        static string CallWebService(string url, string SOAPAction, string soapRequest)
        {
            WebRequest webRequest = WebRequest.Create(url);
            HttpWebRequest httpRequest = (HttpWebRequest)webRequest;
            httpRequest.Method = "POST";
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.Headers.Add("SOAPAction", SOAPAction);
            httpRequest.ProtocolVersion = HttpVersion.Version11;
            httpRequest.Credentials = CredentialCache.DefaultCredentials;
            Stream requestStream = httpRequest.GetRequestStream();
            //Create Stream and Complete Request             
            StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.ASCII);

            streamWriter.Write(soapRequest.ToString());
            streamWriter.Close();
            //Get the Response    
            HttpWebResponse wr = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader srd = new StreamReader(wr.GetResponseStream());
            string resulXmlFromWebService = srd.ReadToEnd();
            return resulXmlFromWebService;
        }
        static String RenewXMLTemplate = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:card=""http://card.bstc.com/"">
               <soapenv:Header/>
               <soapenv:Body>
                  <card:TopUpCard>
                        <User_ID>{0}</User_ID>
                        <Amount>{1}</Amount>
                        <mode>{2}</mode>
                        <provider>{3}</provider>
                        <merchant_code>{4}</merchant_code>
                  </card:TopUpCard>
               </soapenv:Body>
            </soapenv:Envelope>";
        static string Encrypt(string key, string data)
        {
            data = data.Trim();
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0, 24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            tripdes.GenerateIV();
            MemoryStream ms = new MemoryStream();
            CryptoStream encStream = new CryptoStream(ms, tripdes.CreateEncryptor(),
            CryptoStreamMode.Write);
            encStream.Write(Encoding.ASCII.GetBytes(data), 0,
            Encoding.ASCII.GetByteCount(data));
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            ms.Close();
            encStream.Close();
            return Convert.ToBase64String(cryptoByte, 0,
            cryptoByte.GetLength(0)).Trim();
        }
        static string Decrypt(string key, string data)
        {
            byte[] keydata = Encoding.ASCII.GetBytes(key);
            string md5String = BitConverter.ToString(new
            MD5CryptoServiceProvider().ComputeHash(keydata)).Replace("-", "").ToLower();
            byte[] tripleDesKey = Encoding.ASCII.GetBytes(md5String.Substring(0,
            24));
            TripleDES tripdes = TripleDESCryptoServiceProvider.Create();
            tripdes.Mode = CipherMode.ECB;
            tripdes.Key = tripleDesKey;
            byte[] cryptByte = Convert.FromBase64String(data);
            MemoryStream ms = new MemoryStream(cryptByte, 0, cryptByte.Length);
            ICryptoTransform cryptoTransform = tripdes.CreateDecryptor();
            CryptoStream decStream = new CryptoStream(ms, cryptoTransform,
            CryptoStreamMode.Read);
            StreamReader read = new StreamReader(decStream);
            return (read.ReadToEnd());
        }
    }
}