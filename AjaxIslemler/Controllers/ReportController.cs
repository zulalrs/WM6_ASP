using AjaxIslemler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxIslemler.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult Rapor1()
        {
            var db = new NorthwindSabahEntities();
            var sorgu = from dbCategory in db.Categories
                        join dbProduct in db.Products on dbCategory.CategoryID equals dbProduct.CategoryID
                        join dbOrderDetails in db.Order_Details on dbProduct.ProductID equals dbOrderDetails.ProductID
                        group new
                        {
                            dbCategory,
                            dbOrderDetails
                        } by new
                        {
                            dbCategory.CategoryName
                        } into gb
                        select new
                        {
                            gb.Key.CategoryName,
                            Total = gb.Sum(x => x.dbOrderDetails.Quantity)
                        };
            var data = sorgu.ToList();
            return Json(new ResponseData()
            {
                message = $"{data.Count} adet kayıt bulundu",
                success = true,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}