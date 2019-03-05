using IlkMvcSayfam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IlkMvcSayfam.Controllers
{
    public class HomeController : Controller
    {
        static List<Kisi> kisiler = new List<Kisi>()    // Bir kisi listesi oluşturduk ve onun içini kisi tipinde objelerle dolduruyoruz. Static yapmamızın sebebi her seferinde farklı id uretmesini engellemek.
        {
            new Kisi()
            {
                Name="Kamil",
                Surname="Fidil"
            },
              new Kisi()
            {
                Name="Hakkı",
                Surname="Fodul"
            },
                new Kisi()
            {
                Name="Falan",
                Surname="Filan"
            },

        };
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult Ornek() // Metod üzerinde bir şey yazmıyorsa default gette çalışır.
        {
            var data = kisiler; // kisiler listesini data değişkenine atadık.Sayfada hazırlamış oldugumuz bu veriyi kullanacaksak gondermemiz gerekiyor.
            return View(data); // Veriyi view agönderdik. Geriye bir ViewResult tipinde ekran görüntüsü döndürecek. ViewResult tipi actionrsultla aynı. yani burada bir kalıtım var. ViewResult ActionResulttan kalıtım almış.
        }
        public ActionResult Detail(Guid? id)    // Parametre isimleri view deki ile aynı olmalı. Oradan bize id geliyor.Parametre olarak onu yazdık. Orada tanımladıgımız action adımızı burada tanımladık.
        {
            var kisi = kisiler.FirstOrDefault(x => x.Id == id); // id ye göre kisi getirdik.Buradan null da gelebilir.
            if (kisi == null) // Eger null sa
                return RedirectToAction("Ornek");   // Ornek sayfasına geri gitsin.RedirectToAction metodu geri gonderme işlemini gerçekleştiriyor.
            return View(kisi); // Eger null değilse de kisi nesnesini view e gönderecek.
        }
    }
}