using Aspose.Pdf;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WebApplication.Utils
{
    public class ConvertPDF
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public string ConvertFromUrl(long? id, string filename)
        {
            try
            {
                string domain = ConfigControl.DOMAIN + "viewpdf/index/" + id;
                string serverPath = System.Web.Hosting.HostingEnvironment.MapPath("/Data/");
                WebRequest req = WebRequest.Create(domain);
                // Get web page into stream
                using (Stream stream = req.GetResponse().GetResponseStream())
                {
                    // Initialize HTML load options
                    HtmlLoadOptions htmloptions = new HtmlLoadOptions(serverPath)
                    {
                        PageInfo = { Width = 842, IsLandscape = true }
                    };
                    // Load stream into Document object
                    Document pdfDocument = new Document(stream, htmloptions);
                    pdfDocument.CenterWindow = true;
                    pdfDocument.HideWindowUI = true;
                    // Save output as PDF format
                    pdfDocument.Save(serverPath + filename + ".pdf");
                }
                return "/Data/" + filename + ".pdf";
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return "";
            }

        }
    }
}