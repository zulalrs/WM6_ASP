using Admin.BLL.Helpers;
using Admin.BLL.Repository;
using Admin.BLL.Services;
using Admin.Models.Entities;
using Admin.Models.Models;
using Admin.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Admin.Web.UI.Controllers
{
    public class ProductController : BaseController
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            ViewBag.ProductList = GetProductSelectList();   // Ürünleri getiren liste
            ViewBag.CategoryList = GetCategorySelectList(); // Kategorileri getiren liste
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Add(ProductViewModel model)
        {
            if (!ModelState.IsValid) // Eger modelimiz validation işlemlerinden gecemezse product ve category listeleri sadece view e gitsin.
            {
                ViewBag.ProductList = GetProductSelectList();
                ViewBag.CategoryList = GetCategorySelectList();
                return View(model);
            }
            try
            {
                if (model.Product.SupProductId.ToString().Replace("0", "").Replace("-", "").Length == 0)    // Guid den null degeri gelmiyor onun yerine 000-000 gibi bir ifade geliyor onun lengthini 0 yaptık böylece supProductId yi null yapabilecegiz. Zaten böyle bir ifade geliyorsa id yok demektir yani null dır.
                    model.Product.SupProductId = null;

                if (model.PostedFile != null && model.PostedFile.ContentLength > 0)  // Eger PostedFile null gelmiyor ve uzunluğu sıfırdan buyukse bir dosyamız var demektir.
                {
                    var file = model.PostedFile;    //Modelimizin PsotedFile ını bir değişkene atadık
                    string fileName = Path.GetFileNameWithoutExtension(file.FileName); // FileName i aldık.
                    string extName = Path.GetExtension(file.FileName); // ExtensionName ini aldık.
                    fileName += StringHelpers.GetCode();    // FileName in sonuna benzersiz bir code ekledik
                    var klasoryolu = Server.MapPath("~/Upload/"); // Klasör yolunu belirledik
                    var dosyayolu = Server.MapPath("~/Upload/") + fileName + extName;   // Dosya yolunu belirledik

                    if (!Directory.Exists(klasoryolu))  // Klasor yoksa oluşturacak
                        Directory.CreateDirectory(klasoryolu);
                    file.SaveAs(dosyayolu); // Varsa dosyayı kaydedecek

                    WebImage img = new WebImage(dosyayolu); // Kaydettiğimiz image dosyasında bazı duzenlemelri bu nesne ile yaptık.
                    img.Resize(200, 200, false);    // Boyut düzenlemesi
                    //img.AddTextWatermark("Wissen");   // Resmin üzerine yazılacak yazı
                    img.Save(dosyayolu);    // Ve tekrar resmi kaydettik.

                    model.AvatarPath = "/Upload/" + fileName + extName; // Sectiğimiz resmi modelimize atadık.
                }                                                       // model.LastPriceUpdateDate = DateTime.Now;
                    var modelPro = new Product()    // viewden bilgileri gelen modeli Product a cast ediyoruz.
                    {
                        Barcode = model.Product.Barcode,
                        Id = model.Product.Id,
                        BuyPrice = model.Product.BuyPrice,
                        CategoryId = model.Product.CategoryId,
                        ProductName = model.Product.ProductName,
                        ProductType = model.Product.ProductType,
                        SalesPrice = model.Product.SalesPrice,
                        UnitsInStock = model.Product.UnitsInStock,
                        LastPriceUpdateDate = DateTime.Now,
                        SupProductId = model.Product.SupProductId,
                        Quantity = model.Product.Quantity,
                        Description = model.Product.Description,
                        AvatarPath = model.AvatarPath
                    };
                    await new ProductRepo().InsertAsync(modelPro); // Product tipine cevirdiğimiz modelimizi insert ediyoruz.
                    TempData["Message"] = $"{model.Product.ProductName} isimli ürün başarıyla eklenmiştir"; // TempData ile de işlemin basarılı oldugu mesajını view e gonderiyoruz.
                    return RedirectToAction("Add"); // Ekleme sayfasına tekrar donuş yapıyoruz.

                }
            catch (DbEntityValidationException ex)  // Validation hatası geldiğinde çalışacak
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {EntityHelpers.ValidationMessage(ex)}",
                    ActionName = "Add",
                    ControllerName = "Product",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)    // Herhangi bir hata geldiğinde çalışacak
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {ex.Message}",
                    ActionName = "Add",
                    ControllerName = "Product",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpGet]
        public JsonResult CheckBarcode(string barcode)  // Url den gelen barcode parametre olarak kullanılacak
        {
            try
            {
                if (new ProductRepo().Queryable().Any(x => x.Barcode == barcode))   // Eger ProductRepoda gelen barkod varsa
                {
                    var product = new ProductRepo().Queryable().FirstOrDefault(x => x.Barcode == barcode); // O product ı getirdik ve bir değişkene atadık
                    return Json(new ResponseData() // Ve responsedata ile kayıtlı olduguna dair bir mesaj gonderdik
                    {
                        message = $"{barcode} sistemde kayıtlı",
                        success = true,
                        data = new BarcodeResult()  // Datamızı da BarcodeResult tipinde dondurerek bazı bilgilerini ve resmini form üzerinde gormuş olduk
                        {
                            Barcode = product.Barcode,
                            Name = product.ProductName,
                            Price = product.SalesPrice,
                            PhotoUrl = product.AvatarPath
                        }
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new ResponseData()  // Bilgi servisten yani siteden getirildiğinde verilecek mesaj ve dondurülecek data
                {
                    message = $"{barcode} bilgisi servisten getirildi",
                    success = true,
                    data = new BarcodeService().Get(barcode)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)    // Hata oldugunda çalışacak
            {
                return Json(new ResponseData()
                {
                    message = $"Bir hata oluştu: {ex.Message}",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}