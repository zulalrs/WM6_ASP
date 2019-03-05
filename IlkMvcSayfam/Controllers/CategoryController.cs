using IlkMvcSayfam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IlkMvcSayfam.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        // Adres cubuguna yazdığımız actionlar tetiklenir ve sonucunda viewlar gelir.View içerisinde tanımladıgımız actionlar ekrandan tetiklenince buradaki tetiklenen action çalışır.Ve tekrardan view gelir.
        public ActionResult Index() // İlk önce index actionları tetiklenir.
        {
            var data = new NorthwindSabahEntities()
                .Categories
                .OrderBy(x => x.CategoryName)
                .ToList();
            return View(data);
        }
        public ActionResult Detail(int? id) // View den id geliyor
        {
            if (id == null) return RedirectToAction("Index"); // Eger null sa Index sayfasına geri donuyor.
            var data = new NorthwindSabahEntities().Categories.Find(id.Value); // Null değilse gelen id deki kategoriyi getiriyor.
            if (data == null) RedirectToAction("Index"); // Eger data da null gelirse Index sayfasına geri donuyor.

            return View(data); // Gelen datayı dondürüyor.
        }
        [HttpPost] // Annotation ekledik.HtpPost ile çalışacaksın dedik.
        public ActionResult Add(Category category)  // Paremetre olarak; (string CategoryName,string Description)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                db.Categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int? id)
        {
            var db = new NorthwindSabahEntities();
            try
            {
                var category = db.Categories.Find(id.GetValueOrDefault()); // Eger degeri varsa değerini alacak yoksa de default degerini alacak. Categorimizi getirdik
                if (category == null) return RedirectToAction("Index"); // nullsa Index sayfasına git

                db.Categories.Remove(category); // Gelen kategoriyi sil
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Index");
        }
        // Index sayfasında update e tıklandıgı zaman update actionına düşüyor. Index sayfasında url actiondan gelen id yi  de parametre olarak gonderiyoruz. Buradaki veri boylece Update view ına gonderiliyor.
        [HttpGet]
        public ActionResult Update(int? id)
        {
            if (id == null) return RedirectToAction("Index"); //("Index","Home") Sadece tek bir parametre yazdıgımızda bulundugu controllerdaki view a gider. İkinci parametre hangi controllerdaki view a gitmemiz gerektiği
            try
            {
                var data = new NorthwindSabahEntities().Categories.Find(id.Value);  // Gelen id den secilen kategoriyi buluyor.
                if (data == null)   // Data null ise index sayfasına geri gönderiyor.
                    return RedirectToAction("Index");
                return View(data);  //Data geldiyse, category nesnemizin bilgilerini Update adlı sayfaya gönderiyoruz.Ve artık o sayfa görüntüleniyor.
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]  // View ekranında Update tıklanınca çalışacak
        public ActionResult Update(Category model)  // Update view dan bize bir Category nesnesi gelecek.
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var data = db.Categories.Find(model.CategoryID); //Gelen modeli sql de buluyoruz.

                if (data == null) return RedirectToAction("Index");
                //Buldugumuz category nesnesinin ismini ve description ı modelinki ile değiştiriyouz.SaveChanges ile değişiklikleri sql üzerinde kaydediyoruz.
                data.CategoryName = model.CategoryName; 
                data.Description = model.Description;
                db.SaveChanges();
                ViewBag.Message = "<span class='text text-success'>Update Successfuly</span>";  // Ekranda çıkacak mesaj için viewBag kullandık.
                return View(data); // Verimizi tekrar Update sayfasına gönderiyoruz.
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"<span class='text text-danger'>Update Error {ex.Message}</span>";
                return View(model);
            }
        }
        public JsonResult Categories()
        {
            var categoriler = new NorthwindSabahEntities().Categories.Select(x => new
            {
                x.CategoryName,
                x.CategoryID,
                x.Description,
                ProductCount = x.Products.Count
            }).ToList();    // Kategori nesnemizi oluşturduk ve görünüm için select yaptık. Json oluştururken view model kullanmalıyız yoksa hata alabiliriz.Onun için select yapıyrouz.

            return Json(categoriler, JsonRequestBehavior.AllowGet); // Donusumu burada oluşturduk.
        }
       
    }
}