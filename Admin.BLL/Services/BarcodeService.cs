using Admin.Models.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.BLL.Services
{
    public class BarcodeService
    {
        // HtmlAgilityPack paketi kurduğumuzda bir sayfayı html sayfasıymış gibi kullanabiliyoruz.
        public BarcodeResult Get(string barcode)
        {
            var url = $"http://www.barkodid.com/{barcode}";

            try
            {
                var web = new HtmlWeb();    // Yeni bir htmlWeb nesnesi oluşturduk.
                var doc = web.Load(url);    // Load metoduna kullanacagımız sitenin web adresini yazdık.
                var section=doc.DocumentNode.SelectNodes("//div[contains(@class,'product-details')]").FirstOrDefault(); // Sitenin documentinde divler arasında sınıfı product-details  olan div i sectik
                var bc = section?.Element("h1").InnerText.Substring(4); // O divin elementlerinden h1 olanın içeriğinin 4. karakterinden sonrasını aldık.
                var name = section?.Element("h4").InnerText.Trim();     // h4 elementinin içeriğinin boşluklarını atarak aldık
                // Girdiğimiz barcode göre sayfadan ürünün ismini ve fiyatını HtmlAgilityPack yardımı ile sectik ve aldık.
                var priceT = section?.SelectNodes("//div[contains(@class,'product-text')]").FirstOrDefault()?.Element("span").InnerText.Trim();
                var price = Convert.ToDecimal(priceT?.Substring(0, priceT.Length - 3).Replace(".", ","));
                return new BarcodeResult()  // Siteden aldığımız barcode bilgilerini yeni oluşturduğumuz BarcodeResult nesnesindeki propertylere atıyoruz.
                {
                    Barcode = barcode,
                    Name = name,
                    Price = price
                };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
