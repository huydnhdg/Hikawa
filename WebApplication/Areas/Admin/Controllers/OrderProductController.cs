using NLog;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Areas.Admin.Data;
using WebApplication.Models;

namespace WebApplication.Areas.Admin.Controllers
{
    public class OrderProductController : BaseController
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public ActionResult Index(int? page, string textsearch, string chanel, string status, string start_date, string end_date, string im_start_date, string im_end_date)
        {
            var model = from a in db.E_Product
                        select a;
            if (!string.IsNullOrEmpty(textsearch))
            {
                model = model.Where(a => a.Code.StartsWith(textsearch)
                || a.Name.Contains(textsearch)
                || a.Description.Contains(textsearch));
                ViewBag.textsearch = textsearch;
            }
            if (!string.IsNullOrEmpty(chanel))
            {
                model = model.Where(a => a.Cate.ToString() == chanel);
                ViewBag.chanel = chanel;
            }

            if (!string.IsNullOrEmpty(start_date))
            {
                DateTime s = DateTime.ParseExact(start_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                model = model.Where(a => a.Createdate >= s);
                ViewBag.start_date = start_date;
            }
            if (!string.IsNullOrEmpty(end_date))
            {
                DateTime s = DateTime.ParseExact(end_date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                s = s.AddDays(1);
                model = model.Where(a => a.Createdate <= s);
                ViewBag.end_date = end_date;
            }
            ViewBag.cate = db.E_ProductCate.ToList();
            IEnumerable<E_Product> data = model as IEnumerable<E_Product>;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(data.OrderByDescending(a => a.Createdate).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            var cate = db.E_ProductCate;
            ViewBag.cate = cate.ToList();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "")] E_Product_Extra_Create e_Product, IEnumerable<HttpPostedFileBase> Images)
        {
            try
            {
                var _product = db.E_Product.FirstOrDefault(a => a.Code == e_Product.Code);
                if (_product != null)
                {
                    SetAlert("Đã tồn tại mã sản phẩm trong hệ thống.", "danger");
                    return RedirectToAction("Index");
                }
                var product = new E_Product()
                {
                    Createdate = DateTime.Now,
                    Cate = e_Product.Cate,
                    Code = e_Product.Code,
                    Name = e_Product.Name,
                    Link = e_Product.Link,
                    Thumnails = e_Product.Thumnails,
                    Banner = e_Product.Banner,
                    Description = e_Product.Description,
                    ListedPrice = e_Product.ListedPrice,
                    Price = e_Product.Price,
                    Discount = e_Product.Discount,
                    Status = e_Product.Status,
                    IsNew = e_Product.IsNew,
                    Model = e_Product.Model,
                    Trademark = e_Product.Trademark,
                    Unit = e_Product.Unit,
                    Limited = e_Product.Limited,
                    LongDescription = e_Product.LongDescription,
                    Details = e_Product.Details,
                };
                db.E_Product.Add(product);
                db.SaveChanges();

                if (e_Product.Extras.Count() > 0)
                {
                    foreach (var item in e_Product.Extras)
                    {
                        var extra = new E_Product_Extra()
                        {
                            IdProduct = e_Product.Id,
                            Title = item.Title,
                            Description = item.Description
                        };
                        db.E_Product_Extra.Add(extra);
                    }
                    db.SaveChanges();
                }


                foreach (var item in Images)
                {
                    if (item != null)
                    {
                        var rand = new Random();
                        string strrand = rand.Next(0, 999).ToString();
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/ImageProduct/"), strrand + "_" + fileName);
                        item.SaveAs(path);
                        string link = "/ImageProduct/" + strrand + "_" + fileName;

                        var image = new E_Product_Image()
                        {
                            Image = link,
                            Id_Product = e_Product.Id,
                        };
                        db.E_Product_Image.Add(image);
                        db.SaveChanges();
                    }
                }

                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
                return RedirectToAction("Index");
            }

        }
        public ActionResult Edit(long Id)
        {
            var cate = db.E_ProductCate;
            ViewBag.cate = cate.ToList();

            var query = from a in db.E_Product
                        select new E_Product_Extra_Create
                        {
                            Id = a.Id,
                            Cate = a.Cate,
                            Code = a.Code,
                            Name = a.Name,
                            Link = a.Link,
                            Thumnails = a.Thumnails,
                            Banner = a.Banner,
                            Description = a.Description,
                            ListedPrice = a.ListedPrice,
                            Price = a.Price,
                            Discount = a.Discount,
                            Status = a.Status,
                            IsNew = a.IsNew,
                            Model = a.Model,
                            Trademark = a.Trademark,
                            Unit = a.Unit,
                            Limited = a.Limited,
                            LongDescription = a.LongDescription,
                            Details = a.Details,
                            Extras = db.E_Product_Extra.Where(e => e.IdProduct == a.Id).ToList()
                        };
            var model = query.FirstOrDefault(q => q.Id == Id);
            var images = db.E_Product_Image.Where(a => a.Id_Product == Id);
            List<E_Product_Image> list = new List<E_Product_Image>();
            if (images.Count() > 0)
            {
                list = images.ToList();
            }
            ViewBag.images = list;


            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "")] E_Product_Extra_Create e_Product, IEnumerable<HttpPostedFileBase> Images)
        {
            try
            {
                var product = db.E_Product.Find(e_Product.Id);
                product.Cate = e_Product.Cate;
                product.Code = e_Product.Code;
                product.Name = e_Product.Name;
                product.Link = e_Product.Link;
                product.Thumnails = e_Product.Thumnails;
                product.Banner = e_Product.Banner;
                product.Description = e_Product.Description;
                product.ListedPrice = e_Product.ListedPrice;
                product.Price = e_Product.Price;
                product.Discount = e_Product.Discount;
                product.Status = e_Product.Status;
                product.IsNew = e_Product.IsNew;
                product.Model = e_Product.Model;
                product.Trademark = e_Product.Trademark;
                product.Unit = e_Product.Unit;
                product.Limited = e_Product.Limited;
                product.LongDescription = e_Product.LongDescription;
                product.Details = e_Product.Details;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();

                var extra = db.E_Product_Extra.Where(a => a.IdProduct == e_Product.Id).ToList();
                if (extra.Count() > 0)
                {
                    var countId = extra.OrderByDescending(a => a.ID).FirstOrDefault();
                    if (e_Product.Extras.Count() > extra.Count())
                    {
                        //cố tính chạy từ dong mới nhất
                        for (var i = extra.Count(); i < e_Product.Extras.Count(); i++)
                        {
                            var newExtra = new E_Product_Extra()
                            {
                                Title = e_Product.Extras[i].Title,
                                Description = e_Product.Extras[i].Description,
                                IdProduct = e_Product.Id
                            };
                            db.E_Product_Extra.Add(newExtra);
                            db.SaveChanges();

                        }
                    }

                }
                else
                {
                    for (var i = 0; i < e_Product.Extras.Count(); i++)
                    {
                        var newExtra = new E_Product_Extra()
                        {
                            Title = e_Product.Extras[i].Title,
                            Description = e_Product.Extras[i].Description,
                            IdProduct = e_Product.Id,
                        };
                        db.E_Product_Extra.Add(newExtra);
                        db.SaveChanges();
                    }
                }


                foreach (var item in Images)
                {
                    if (item != null)
                    {
                        var rand = new Random();
                        string strrand = rand.Next(0, 999).ToString();
                        var fileName = Path.GetFileName(item.FileName);
                        var path = Path.Combine(Server.MapPath("~/Data/ImageProduct/"), strrand + "_" + fileName);
                        item.SaveAs(path);
                        string link = "/Data/ImageProduct/" + strrand + "_" + fileName;

                        var image = new E_Product_Image()
                        {
                            Image = link,
                            Id_Product = e_Product.Id,
                        };
                        db.E_Product_Image.Add(image);
                        db.SaveChanges();
                    }
                }
                SetAlert("Đã lưu thông tin thành công.", "success");
                return RedirectToAction("Edit", "OrderProduct", new { Id = e_Product.Id });
            }
            catch (Exception ex)
            {
                SetAlert("Lưu thông tin không thành công. Hãy kiểm tra lại.", "danger");
                return RedirectToAction("Edit", "OrderProduct", new { Id = e_Product.Id });
            }

        }
        public ActionResult BackExtra(long Id)
        {
            var extra = db.E_Product_Extra.Find(Id);
            db.E_Product_Extra.Remove(extra);
            db.SaveChanges();
            return RedirectToAction("Edit", "OrderProduct", new { Id = extra.IdProduct });
        }
        public ActionResult DeleteImage(long Id)
        {
            var image = db.E_Product_Image.Find(Id);

            var images = db.E_Product_Image.Where(a => a.Id_Product == image.Id_Product);
            List<E_Product_Image> list = new List<E_Product_Image>();
            if (images.Count() > 0)
            {
                list = images.ToList();
            }
            ViewBag.images = list;


            db.E_Product_Image.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Edit", "OrderProduct", new { Id = image.Id_Product });
        }
        public ActionResult Delete(long Id)
        {
            try
            {
                var model = db.E_Product.Find(Id);
                db.E_Product.Remove(model);
                db.SaveChanges();
                SetAlert("Đã xóa thành công.", "success");
            }
            catch (Exception ex)
            {
                SetAlert(ex.Message, "danger");
            }

            return RedirectToAction("Index");
        }

    }
}