using AjaxIslemler.Models;
using AjaxIslemler.Models.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjaxIslemler.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index() // Bu actionın işi sadece index viewı çalıştırmak olacak.
        {
            return View();
        }
        [HttpGet]
        public JsonResult Search(string s)  // Ajax işlemlerinde bize jason nesnesi lazım oldugu için JsonResult tipinde deger donduren bir action yazdık. Bu action metodu string tipinde bir deger alacak ve o deger s parametresine aktarılacak.
        {
            var key = s.ToLower();  // Küçük büyük harf duyarlılığı oldugu için gelen degeri küçük harfe donduruyoruz.
            if (key.Length <= 2 && key != "*")   // key degerinin uzunlugu 3 ten küçük ve * a esit değilse herhangi bir veri dondurmeyecek. sadece uyarı mesajı verecek.
            {
                //JsonResult return de bizden iki parametre istiyor.Birincisi object data ve onun için response data nesnemizi gönderiyoruz. İkincisi de Json behavior. Bütün Json verilerimizi ResponseData yapısında dondurecegız.
                return Json(new ResponseData()
                {
                    //Herhangi bir data dondurmeyecegimiz için burada data propertyimizi almadık.
                    message = "Aramak için 2 karakterden fazlasını girin",    // message propertyimizi burada dolduruyoruz.
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                var db = new NorthwindSabahEntities();
                List<CategoryViewModel> data;

                if (key == "*") // Url action bu  metodu tetikliyor eger key bize url actiondan * olarak gelirse bu kısım calışacak yani tüm kategoriler gelecek.Category view model nesnemizi gelen her deger ile doldurup buradan geri gönderiyoruz.
                {
                    data = db.Categories.OrderBy(x => x.CategoryName)
                        .Select(x => new CategoryViewModel()
                        {
                            CategoryName = x.CategoryName,
                            Description = x.Description,
                            CategoryID = x.CategoryID,
                            ProductCount = x.Products.Count
                        })
                        .ToList();
                }
                else
                {
                    // Arama motorundan girilen degerlere göre çalışacak sorgumuz.CategoryName ve Description a bakacak. key degerini içeren category nesnelerini getirecek.Bu sorgudan liste gelir.
                    data = db.Categories
                        .Where(x =>
                        x.CategoryName.ToLower().Contains(key) || x.Description.Contains(key))
                        .Select(x => new CategoryViewModel()
                        {
                            CategoryName = x.CategoryName,
                            Description = x.Description,
                            CategoryID = x.CategoryID,
                            ProductCount = x.Products.Count
                        })
                        .ToList();
                }
                // Eger herhangi bir hata olmazsa buradaki Json degeri donecek. Ve artık bu aşamadan sonra elimizde bir data olacagı için data propertyimizi de burada doldurmuş olduk.
                return Json(new ResponseData()
                {
                    message = $"{data.Count} adet kayıt b" +
                    $"ulundu",
                    success = true,
                    data = data // View e geri dondurulecek data burada responsedatanın data propertysine aktarılıyor ve buradan view de gonderildiği yere geri gidiyor.
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Herhangi bir hata durumunda buradaki Json donecek.
                return Json(new ResponseData()
                {
                    message = $"Bir hata oluştu {ex.Message}",
                    success = false
                }, JsonRequestBehavior.AllowGet);
            }
           
        }

        [HttpPost] // Aşagıdaki metod post ile çalışacak
        public JsonResult Add(CategoryViewModel model)  // Textboxlara girilen deger ajax ile buradaki model e gonderiliyor. // Burada her isinde de kullanacagımız aynı propertyler olduğu için model i Category tipinde de alsak olurdu.
        {
            try
            {
                var db = new NorthwindSabahEntities();
                db.Categories.Add(new Category() {  // Gelen model nesnesini yeni bir kategori olarak veritabanına ekliyoruz.
                    CategoryName=model.CategoryName,
                    Description=model.Description
                });
                db.SaveChanges();
                return Json(new ResponseData()  // En son Responsedata mızı mesaj ve success kısmını degiştirerek buradan geri donduruyoruz.
                {
                    message = $"{model.CategoryName} ismindeki kategori başarıyla eklendi",
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
        public JsonResult Delete(int id)    // Ajax tan gelen id buradaki id parametresine atandı ve o id deki categori bulunup silindi.
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var cat = db.Categories.Find(id);
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    message = $"{cat.CategoryName} ismindeki kategori başarıyla silindi",
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
                throw;
            }
        }

        [HttpPost]
        public JsonResult Update(Category model)    // Ajax tan gelen data buradaki model parametresine atandı. model in parametresi ile o nesneyi sql de bulduk ve model in propertylerini o nesnenin propertylerine atadık.
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var cat = db.Categories.Find(model.CategoryID);
                if (cat == null)    // Eger null ise sadece mesaj donduruyoruz.
                {
                    return Json(new ResponseData()
                    {
                        message = $"Kategori bulunamadı",
                        success = false
                    }, JsonRequestBehavior.AllowGet);
                }
                // Atama işlemlerini yapıp değişiklikleri kaydettik.
                cat.Description = model.Description;
                cat.CategoryName = model.CategoryName;
                db.SaveChanges();
                return Json(new ResponseData()
                {
                    message = $"{cat.CategoryName} ismindeki kategori başarıyla guncellendi",
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


    }
}