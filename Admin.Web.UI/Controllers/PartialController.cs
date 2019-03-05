using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Web.UI.Controllers
{
    public class PartialController : Controller
    {
        // GET: Partial
        public PartialViewResult DrawerPartial()    // Layouttan burası tetiklenecek ve buradan drawerpartial viewına gidecek
        {
            var data = new List<string>();
            return PartialView("Partial/_DrawerPartial", data); //Bu metod layouttan çağrıldığında DrawerPartialview goruntusu return ile layoutta geri dönecek ve bu view layout içerisinde görünmüş olacak böylece drawer kısmı partial view ile taşınmıs oluyor.Partial view ile data(model) de gonderebiliyoruz.
        }
        public PartialViewResult HeaderPartial()    // HeaderPartial viewini donduren metod
        {
            return PartialView("Partial/_HeaderPartial");
        }
        public PartialViewResult ModalPartial()    // ModalPartial viewini donduren metod
        {
            return PartialView("Partial/_ModalPartial");
        }
    }
}