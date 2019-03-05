using AjaxIslemler.Models;
using AjaxIslemler.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxIslemler.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetAllCategories()
        {
            // ResponseData kullanmamızın sebebi gelen ve giden jsonımızın bir standartının olması. İçinde belli propeertyler var onları javascripte karıstırmamak için belli bir standart belirledikgetirmek için yazdığımız JsonResult metodu.
            // Tüm kategorileri 
            try
            {
                var db = new NorthwindSabahEntities();
                var data = db.Categories.Select(x => new CategoryViewModel()
                {
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    CategoryID = x.CategoryID,
                    ProductCount = x.Products.Count
                });
                return Json(new ResponseData()
                {
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    success = false,
                    message = $"Bir hata oluştu {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAllProducts(string key)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var query = db.Products.AsQueryable();  //AsQueryable() yazmazsak query degğişkenimiz dbset tipinde oluyor. Bİz aynı sorgu üzerinden arama kosulunu yazmak istediğimiz ve devamında da kullanmak istediğimiz için bu değişkenimizi AsQueryable ile Queryable tipine donuşturduk. Böylece aynı değişkeni her şekilde kullanabildik. if e girsede girmesede iften sonraki satırda bu değişkenin queryable tipinde olması lazım.
                if (!string.IsNullOrEmpty(key)) // Eger key parametresi boş gelmezse arama sartını gercekleştirecek.
                {
                    key = key.ToLower();
                    query = query.Where(x => x.ProductName.ToLower().Contains(key) ||
                                       x.Category.CategoryName.ToLower().Contains(key) ||
                                       x.Supplier.CompanyName.ToLower().Contains(key));
                }
                var data = query.OrderBy(x => x.ProductName)
                    .ToList()
                    .Select(x => new ProductViewModel()
                    {
                        CategoryName = x.Category?.CategoryName,    // CategoryName nullable bir alan. Bu ifadeyi çğırdığımızda bize null gelme durumu olabilir.Bunun için ?. kullandık.Null gelirse bu satırı işlemeyecek. Ama bu lambda expressinda null propagationı yapamıyor.Çünkü biz burada sadece sorguyu hazırlıyoruz ve bu sorgu tolist demeden sql e atılmış oluyor.Böyle bir ifedenin karşılıgıda sql de olmadığı için hata veriyor. Bunu önlemek için selectten once tolist yazdık.Boylece liste ram e geldi ve burada artık c# komutları çalışacak. ve ifadenin C# ta karşılığı oldugu için hata da ortadan kalkacak. 
                        AddedDate = x.AddedDate,
                        CategoryID = x.CategoryID,
                        ProductName = x.ProductName,
                        UnitsInStock = x.UnitsInStock,
                        UnitPrice = x.UnitPrice,
                        ProductID = x.ProductID,
                        AddedDateFormatted = $"{x.AddedDate:g}",    // Formattlanmış halini buradan gonderiyoruz.Ekranda da bunu gösterecegiz.
                        Discontinued = x.Discontinued,
                        QuantityPerUnit = x.QuantityPerUnit,
                        ReorderLevel = x.ReorderLevel,
                        SupplierID = x.SupplierID,
                        SupplierName = x.Supplier?.CompanyName,
                        UnitPriceFormatted = $"{x.UnitPrice:c2}",   // Formattlanmış halini buradan gonderiyoruz.Ekranda da bunu gösterecegiz.
                        UnitsOnOrder = x.UnitsOnOrder
                    })
                    .ToList();
                return Json(new ResponseData()
                {
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    success = false,
                    message = $"Bir hata olustu {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Add(Product model)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                model.CategoryID = model.CategoryID == 0 ? null : model.CategoryID; // Gelen modelin categoryid si 0 ise null olarak donmesi lazım onun için boyle bir şey yazdık.
                model.AddedDate = DateTime.Now; // AddedDate in null olmaması lazım onun için onun da degerini burada atadık.
                db.Products.Add(model);
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    success = true,
                    message = $"{model.ProductName} isimli ürün basariyla eklenmiştir."
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    success = false,
                    message = $"Bir hata olustu {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Index2

        [HttpGet]
        public JsonResult GetAllProducts2(int? id)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var data = db.Products.Where(x => x.CategoryID == id)
                    .OrderBy(x => x.ProductName)
                    .ToList()
                    .Select(x => new ProductViewModel()
                    {
                        CategoryName = x.Category?.CategoryName,
                        AddedDate = x.AddedDate,
                        CategoryID = x.CategoryID,
                        ProductName = x.ProductName,
                        UnitsInStock = x.UnitsInStock,
                        UnitPrice = x.UnitPrice,
                        ProductID = x.ProductID,
                        AddedDateFormatted = $"{x.AddedDate:g}",
                        Discontinued = x.Discontinued,
                        QuantityPerUnit = x.QuantityPerUnit,
                        ReorderLevel = x.ReorderLevel,
                        SupplierID = x.SupplierID,
                        SupplierName = x.Supplier?.CompanyName,
                        UnitPriceFormatted = $"{x.UnitPrice:c2}",
                        UnitsOnOrder = x.UnitsOnOrder
                    })
                    .ToList();
                return Json(new ResponseData()
                {
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    success = false,
                    message = $"Bir hata oluştu {ex.Message}"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Add2(ProductViewModel model)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                db.Products.Add(new Product()
                {
                    ProductName = model.ProductName,
                    CategoryID = model.CategoryID,
                    UnitPrice = model.UnitPrice,
                    UnitsInStock = model.UnitsInStock,
                    AddedDate = model.AddedDate,
                    Discontinued = model.Discontinued,
                    QuantityPerUnit = model.QuantityPerUnit,
                    ReorderLevel = model.ReorderLevel,
                    SupplierID = model.SupplierID,
                    UnitsOnOrder = model.UnitsOnOrder
                });
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    message = $"{model.ProductName} ismindeki urun başarıyla eklendi",
                    success = true,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    message = $"Bir hata oluştu {ex.Message}",
                    success = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update2(Product model)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var product = db.Products.Find(model.ProductID);
                if (product == null)
                {
                    return Json(new ResponseData()
                    {
                        message = $"Urun bulunamadı",
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
                product.ProductName = model.ProductName;
                product.QuantityPerUnit = model.QuantityPerUnit;
                product.CategoryID = model.CategoryID;
                product.UnitsInStock = model.UnitsInStock;
                product.UnitPrice = model.UnitPrice;
                product.UnitsOnOrder = model.UnitsOnOrder;
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    message = $"{product.ProductName} ismindeki kategori başarıyla guncellendi",
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    message = $"Bir hata oluştu {ex.Message}",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Delete2(int id)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var product = db.Products.Find(id);
                db.Products.Remove(product);
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    message = $"{product.ProductName} ismindeki kategori başarıyla silindi",
                    success = true
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    message = $"Kategori silme işleminde hata {ex.Message}",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Search2(string s)
        {
            var key = s.ToLower();
            if (key.Length <= 2 && key != "*")
            {
                return Json(new ResponseData()
                {
                    message = "Aramak için 2 karakterden fazlasını girin",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var db = new NorthwindSabahEntities();
                List<ProductViewModel> data;
                if (key == "*")
                {
                    data = db.Products.OrderBy(x => x.ProductName)
                        .ToList()
                    .Select(x => new ProductViewModel()
                    {
                        CategoryName = x.Category?.CategoryName,
                        AddedDate = x.AddedDate,
                        CategoryID = x.CategoryID,
                        ProductName = x.ProductName,
                        UnitsInStock = x.UnitsInStock,
                        UnitPrice = x.UnitPrice,
                        ProductID = x.ProductID,
                        AddedDateFormatted = $"{x.AddedDate:g}",
                        Discontinued = x.Discontinued,
                        QuantityPerUnit = x.QuantityPerUnit,
                        ReorderLevel = x.ReorderLevel,
                        SupplierID = x.SupplierID,
                        SupplierName = x.Supplier?.CompanyName,
                        UnitPriceFormatted = $"{x.UnitPrice:c2}",
                        UnitsOnOrder = x.UnitsOnOrder
                    })
                    .ToList();
                }
                else
                {
                    data = db.Products
                    .Where(x =>
                    x.ProductName.ToLower().Contains(key))
                    .ToList()
                    .Select(x => new ProductViewModel()
                    {
                        CategoryName = x.Category?.CategoryName,
                        AddedDate = x.AddedDate,
                        CategoryID = x.CategoryID,
                        ProductName = x.ProductName,
                        UnitsInStock = x.UnitsInStock,
                        UnitPrice = x.UnitPrice,
                        ProductID = x.ProductID,
                        AddedDateFormatted = $"{x.AddedDate:g}",
                        Discontinued = x.Discontinued,
                        QuantityPerUnit = x.QuantityPerUnit,
                        ReorderLevel = x.ReorderLevel,
                        SupplierID = x.SupplierID,
                        SupplierName = x.Supplier?.CompanyName,
                        UnitPriceFormatted = $"{x.UnitPrice:c2}",
                        UnitsOnOrder = x.UnitsOnOrder
                    })
                    .ToList();
                }
                return Json(new ResponseData()
                {
                    message = $"{data.Count} adet kayıt bulundu",
                    success = true,
                    data = data
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ResponseData()
                {
                    message = $"Bir hata oluştu {ex.Message}",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}