using Admin.BLL.Helpers;
using Admin.BLL.Repository;
using Admin.Models.Entities;
using Admin.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Web.UI.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            ViewBag.CategoryList = GetCategorySelectList(); // ViewBag ile  CategoryList diye bir nesne taşıyoruz ve bu nesne GetCategorySelectList metodundan dönen liste ile dolu.
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        // Post ile çalışan bir metod ve Category tipinde bir parametre alıyor bu parametre view kısmından submit butonu ile gonderiliyor.
        public ActionResult Add(Category model)
        {
            try
            {
                if (model.SupCategoryId == 0)   // Eger üst kategori id si 0 olan secili ise, ki bunu biz GetCategorySelectList metodunda tanımlamıştık, o id yi null yap.Böylece supCateoryId si null olanlar en üst kategori olmu oluyor.
                    model.SupCategoryId = null;
                if (!ModelState.IsValid) // ModelState yani gelen modelimiz gerekli şartları saglamıyorsa
                {
                    ModelState.AddModelError("CategoryName", "100 karakteri geçme");    // Categoryname ile ilgili hata mesajı verir. Bu metodun ilk parametresi hangi alanın hata mesajı olacagı, ikinci parametresi ise hata mesajı
                    model.SupCategoryId = model.SupCategoryId ?? 0; // Modelin supcategoryId si varsa onu al yoksa da 0 al
                    ViewBag.CategoryList = GetCategorySelectList(); // Category listemizi getir
                    return View(model); // Ve modeli tekrar view e gonder.
                }

                if (model.SupCategoryId > 0)   // Eger üst kategori idsi 0 dan buyukse o zaman nesne alt kategoridedir. 
                {
                    model.TaxRate = new CategoryRepo().GetById(model.SupCategoryId).TaxRate;    // modeli supcategoryid sine göre repoda ara ve onun kdv sini alt category olan bu nesneye ata.
                }
                new CategoryRepo().Insert(model);   // modelimizi yani yeni categorymizi insert et
                TempData["Message"] = $"{model.CategoryName} isimli kategori başarıyla eklenmiştir.";   // Tempdata ile mesaj gonder bu mesaj için view kısmında nerede gozukmesini istiyorsak oraya bir tag elementi arasına yazmalıyız.
                return RedirectToAction("Add"); // Add sayfasına tekrardan dondur.
            }
            catch (DbEntityValidationException ex)
            {
                // TempData ya yeni bir errorviewmodel nesnesi atıyoruz
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {EntityHelpers.ValidationMessage(ex)}",   // Hata mesajımız
                    ActionName = "Add", // Hatanın oluştugu ActionName
                    ControllerName = "Category", // Ve hangi controllerda oldugu
                    ErrorCode = 500 // Hata kodu
                };
                return RedirectToAction("Error", "Home");   // Hata olması durumunda gideceği sayfa
            }
            catch (Exception ex)
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {ex.Message}",
                    ActionName = "Add",
                    ControllerName = "Category",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id = 0)  // id null gelirse 0 alacak, null gelmezsede url ye yazılan id yi alacak. Update sayfasını id girilmeden çalıştırılması halinde çıkacak hataları onlemek için parametre de böyle bir tanımlama yaptık.
        {
            ViewBag.CategoryList = GetCategorySelectList(); // Sayfa açıldığında ilk olaraka kategori listesini getiriyoruz.
            var data = new CategoryRepo().GetById(id);  // id den gelen kategoriyi buluyoruz.

            if (data == null)   // Eger herhangi bir kategori gelmezse hata sayfasına yonlendiriyoruz
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = "Kategori bulunamadı",
                    ActionName = "Add",
                    ControllerName = "Category",
                    ErrorCode = 404
                };
                return RedirectToAction("Error", "Home");
            }
            return View(data);  // Herhangi bir hata olmaması durumunda buldugumuz datayı update viewine gonderiyoruz
        }

       
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(Category model)  // Viewden gelen değişmiş Categoryi parametre olarak alıyoruz
        {
            try
            {
                if (model.SupCategoryId == 0)   // Eger gelen modelin supcategoryid si 0 ise üst kategorisi yoktur. Üst kategorisi olmayan kategorilerede null degeri atıyoruz.
                    model.SupCategoryId = null;

                if (!ModelState.IsValid)    // Eger model, validation kontrollerinden gecemezse
                {
                    model.SupCategoryId = model.SupCategoryId ?? 0; // Supcategoryid varsa onu al yoksa 0 al
                    ViewBag.CategoryList = GetCategorySelectList(); // Listeyi getir
                    return View(model); // Modeli tekrar view e gonder
                }

                if (model.SupCategoryId > 0)    // modelin supcategoryid si 0 dan buyukse alt kategorileri var demektir.
                {
                    model.TaxRate = new CategoryRepo().GetById(model.SupCategoryId).TaxRate;    // Eger oyleyse modelin üst kategorisinin kdv oranını modelin kdv oranına ekle.
                }

                var data = new CategoryRepo().GetById(model.Id);    // Modeli id den bulduk data değişkenine atadık.
                // Yeni deger atamalarını yaptık
                data.CategoryName = model.CategoryName;
                data.TaxRate = model.TaxRate;
                data.SupCategoryId = model.SupCategoryId;
                new CategoryRepo().Update(data);    // Modelin sql tarafında guncellemelerini yaptık.

                foreach (var dataCategory in data.Categories)   // data nın alt kategorileri içinde geziyoruz.
                {
                    dataCategory.TaxRate = data.TaxRate;    // Alt kategorilerinin kdv oranının her birine datanın kdv oranını atıyoruz.
                    new CategoryRepo().Update(dataCategory);    // Herbir alt kategorinin alt kategorilerinin olup olmadıgına bakıyoruz
                    if (dataCategory.Categories.Any()) // Eger varsa 
                        UpdateSubTaxRate(dataCategory.Categories); // Buradaki metodu çağırıyoruz ve parametre olarak donguden gelen alt kategorinin alt kategorilerini veriyoruz.
                }

                void UpdateSubTaxRate(ICollection<Category> dataC)  // Bu metodla beraber her bir alt kategorinin, alt kategorilerinin kdv oranlarına bir üsteki kategorinin kdv oranını atamış oluyoruz.
                {
                    foreach (var dataCategory in dataC)
                    {
                        dataCategory.TaxRate = data.TaxRate;
                        new CategoryRepo().Update(dataCategory);
                        if (dataCategory.Categories.Any())
                            UpdateSubTaxRate(dataCategory.Categories);
                    }
                }
                TempData["Message"] = $"{model.CategoryName} isimli kategori başarıyla güncellenmiştir"; // Güncelleme işlemi başarılı birşekilde gercekleştiğinde view e gondeilecek mesaj.  
                ViewBag.CategoryList = GetCategorySelectList(); // Kategorileri getirecek
                return View(data);  // Güncellenmiş data View e gonderilecek.
            }
            catch (DbEntityValidationException ex)  // Validation hatası oldugunda çalışacak ve hata sayfasına gidecek. Orada hata mesajını gonderecek
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {EntityHelpers.ValidationMessage(ex)}",
                    ActionName = "Add",
                    ControllerName = "Category",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
            catch (Exception ex)    // Bir hata oluştuğunda hata sayfasına burada yazılan mesajı gonderecek. Buradaki Action ve Controller nameleri geri gideceği sayfayı belirlemek için.
            {
                TempData["Model"] = new ErrorViewModel()
                {
                    Text = $"Bir hata oluştu: {ex.Message}",
                    ActionName = "Add",
                    ControllerName = "Category",
                    ErrorCode = 500
                };
                return RedirectToAction("Error", "Home");
            }
        }
    }
}