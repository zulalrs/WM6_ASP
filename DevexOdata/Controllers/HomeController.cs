using DevexOdata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DevexOdata.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About() // About sekmesine tıkladığımızda veri üretecek. Sadece about viewın ekranını gösterecek, ekstra birşey göstermeyecek .
        {
            ViewBag.Message = "Your application description page.";

            ApplicationDbContext db = new ApplicationDbContext(); // Context nesnemize eriştik.
            for (int i = 0; i < 5000; i++) 
            {
                db.Customers.Add(new Customer() // Customer tablomuza, fake data kullanarak Costumer tipinde yeni nesneler ekledik.
                {
                    // Propertyleri doldurduk. 
                    Name = FakeData.NameData.GetFirstName(),
                    Address = FakeData.PlaceData.GetAddress(),
                    Balance = FakeData.NumberData.GetNumber(1250, 99999),
                    Phone = "05" + FakeData.PhoneNumberData.GetPhoneNumber().Replace("-", "").Substring(0, 10),
                    Surname = FakeData.NameData.GetSurname()
                });
                if (i % 100 == 0) // Tek bir seferde 5000 tane ekleyemiyor onun için her 100 taneden sonra dbsavechanges diyerek ekleme yapmış oluyoruz.
                    db.SaveChanges();
            }

            db.SaveChanges();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}