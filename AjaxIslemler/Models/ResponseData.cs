using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjaxIslemler.Models
{
    public class ResponseData
    {
        // JsonResult  gonderimi yaparken kullanabilecegimiz yapı.
        // Property isimlerini küçük harflerle yazmamızın sebebi javascript ile kullanılırken sıkıntı çıkmaması. İlk üç propertyi kullanacagız. Diğer iki parametre kendi atanacak.
        public string message { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
        public DateTime responseTime { get; set; } = DateTime.Now;
        public string responseTimeU { get; set; } = $"{DateTime.Now:O}";    // String formatı içerisinde yazdığımız O tagı Datentime ın universal sistemde olmasını saglıyor.
    }
}