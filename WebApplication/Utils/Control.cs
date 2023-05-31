using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication.Utils
{
    public class Control
    {
        public static string CreateCode(long Id)
        {
            string index = Id.ToString().PadLeft(6, '0');
            string code = "PBH" + index;
            return code;
        }
        public static string CreatePIN()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 999999);
            string index = randomNumber.ToString().PadLeft(6, '0');
            return index;
        }
        public static string CreateCodeOrder(int Id)
        {
            int index = Id++;
            //string index = Id.ToString().PadLeft(6, '0');
            string time = DateTime.Now.ToString("ddMMyy");
            string code = "OD" + time + index;
            return code;
        }
        public static String getMobileOperator(String mobileNumber)
        {

            if (mobileNumber.StartsWith("+"))
            {
                mobileNumber = mobileNumber.Substring(1);
            }
            if (!mobileNumber.StartsWith("84") || (mobileNumber.StartsWith("84") && mobileNumber.Length == 9))
            {
                mobileNumber = formatUserId(mobileNumber, 0);
            }
            if (mobileNumber.StartsWith("8498") || mobileNumber.StartsWith("8497")
                    || mobileNumber.StartsWith("8496")
                    || mobileNumber.StartsWith("8486") || mobileNumber.StartsWith("8432")
                    || mobileNumber.StartsWith("8433") || mobileNumber.StartsWith("8434")
                    || mobileNumber.StartsWith("8435")
                    || mobileNumber.StartsWith("8436") || mobileNumber.StartsWith("8437")
                    || mobileNumber.StartsWith("8438") || mobileNumber.StartsWith("8439")
                    )
            {
                return "VIETEL";
            }
            else if (mobileNumber.StartsWith("8490") || mobileNumber.StartsWith("8493")
                  || mobileNumber.StartsWith("8489")
                  || mobileNumber.StartsWith("8470") || mobileNumber.StartsWith("8476")
                  || mobileNumber.StartsWith("8477") || mobileNumber.StartsWith("8478")
                  || mobileNumber.StartsWith("8479")
                  )
            {
                return "VMS";
            }
            else if (mobileNumber.StartsWith("8491") || mobileNumber.StartsWith("8494")
                  || mobileNumber.StartsWith("8488")
                  || mobileNumber.StartsWith("8481") || mobileNumber.StartsWith("8482")
                  || mobileNumber.StartsWith("8483") || mobileNumber.StartsWith("8484")
                  || mobileNumber.StartsWith("8485") || mobileNumber.StartsWith("8488") || mobileNumber.StartsWith("8487")
                  )
            {
                return "GPC";
            }
            else if (mobileNumber.StartsWith("8492") || mobileNumber.StartsWith("8456")
                 || mobileNumber.StartsWith("8458") || mobileNumber.StartsWith("8452")
                 )
            {
                return "VNM";
            }
            else if (mobileNumber.StartsWith("8499")
                 || mobileNumber.StartsWith("8459"))
            {
                return "GTEL";


            }

            else
            {
                return "UNKNOWN";
            }
        }
        public static String formatUserId(String userId, int formatType)
        {
            String temp = userId;
            switch (formatType)
            {
                case 0://Constants.USERID_FORMAT_INTERNATIONAL:
                    if ((temp.StartsWith("9") || temp.StartsWith("8") || temp.StartsWith("7") || temp.StartsWith("5") || temp.StartsWith("3")) && temp.Length == 9)
                    {
                        temp = "84" + temp;
                    }
                    else if (temp.StartsWith("1") && temp.Length == 10)
                    {
                        temp = "84" + temp;
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = "84" + temp.Substring(1);
                    } // els  
                    break;
                case 1://Constants.USERID_FORMAT_NATIONAL_NINE:
                    if (temp.StartsWith("84"))
                    {
                        temp = temp.Substring(2);
                    }
                    else if (temp.StartsWith("0"))
                    {
                        temp = temp.Substring(1);
                    } // else startsWith("9")
                    break;
                case 2://Constants.USERID_FORMAT_NATIONAL_ZERO:
                    if (temp.StartsWith("84"))
                    {
                        temp = "0" + temp.Substring(2);
                    }
                    else if (!temp.StartsWith("0"))
                    {
                        temp = "0" + temp;
                    } // else startsWith("09")
                    break;
                default:

                    return null;
            }
            return temp;
        }

        public static string CreateAExportCode(long Id)
        {
            string index = Id.ToString("D4");
            string code = "PX" + index;
            return code;
        }
        public static string CREATE_CODE_EXPORT(int index)
        {
            index++;
            string date = DateTime.Now.ToString("yyMMdd");
            string _index = index.ToString().PadLeft(6, '0');
            return "XK" + date + _index;
        }
        public static string CREATE_CODE_IMPORT(int index)
        {
            index++;
            string date = DateTime.Now.ToString("yyMMdd");
            string _index = index.ToString().PadLeft(6, '0');
            return "NK" + date + _index;
        }
    }
}