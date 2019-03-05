using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.Models
{
    // barkodid sitesinden hazır barkod ve ürün bilgileri çekerken kullandığımız sınıf
    public class BarcodeResult
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PhotoUrl { get; set; }
    }
}
