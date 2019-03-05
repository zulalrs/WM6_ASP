using Admin.BLL.Repository;
using Admin.Models.Entities;
using Admin.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Web.UI.Controllers
{
    [Authorize] // Sadece login olan kişi bu sınıftan türetilen controllerların indexlerini görebilir.
    // BaseController dan kalıtım almış tüm ekranlar mutlaka giriş yapılmış olan ekranlar olmak zorunda olacak.
  // [RequireHttps]
    public class BaseController : Controller
    {
        protected List<SelectListItem> GetCategorySelectList()
        {
            // Html deki select option için selectlistitem tipinde bir listeye ihtiyacımız var
            var categories = new CategoryRepo() // Üst kategorisi olmayan categorileri getirdik. Yani null olanları
                .GetAll(x => x.SupCategoryId == null)
                .OrderBy(x => x.CategoryName);
            var list = new List<SelectListItem>()
            {
                // SelectListItem tipide bir liste oluşturduk ve ilk elemanı SelectListItem tipinde bir nesne ve propertylerinin ikisini aşagıda doldurduk
                new SelectListItem()
                {
                    Text="Üst Kategorisi Yok",
                    Value="0"
                }
            };

            foreach (var category in categories)    // Gelen kategori nesnelerini sırayla geziyoruz
            {
                if (category.Categories.Any()) // Nesenin alt kategorileri varsa
                {
                    // Once nesneyi listeye ekliyoruz
                    list.Add(new SelectListItem()
                    {
                        Text = category.CategoryName,
                        Value = category.Id.ToString()    // Bizden string istediği için cevirme yaptık
                    });

                    // Sonra alt kategorileri eklemek için GetSubCategories metodunu çağırdık ve parametre olarakta alt kategorileri yolladık.
                    list.AddRange(GetSubCategories(category.Categories.OrderBy(x => x.CategoryName).ToList()));
                }
                else    // Nesnenin alt kategorileri yoksa oldugu gibi nesneyi ekliyoruz.
                {
                    list.Add(new SelectListItem()
                    {
                        Text = category.CategoryName,
                        Value = category.Id.ToString()
                    });
                }
            }

            // Metod içinde tekrar metod çağırdık(GetSubCategories). C# 8 de gelen bir özellik.
            List<SelectListItem> GetSubCategories(List<Category> categories2)
            {
                var list2 = new List<SelectListItem>(); // Yeni bir SelectListItem tipinde list nesnesi oluşturduk.
                // Gelen listedeki her bir elemanı yine gezeceğiz
                foreach (var category in categories2)
                {
                    if (category.Categories.Any()) // Eger gelen nesneninde alt kategorileri varsa once nesneyi ekleyip daha sonra alt kategorileri eklemek için yine aynı metodu çağırıyoruz (Recersive işlemi)
                    {
                        list2.Add(new SelectListItem()
                        {
                            Text = category.CategoryName,
                            Value = category.Id.ToString()
                        });
                        list2.AddRange(GetSubCategories(category.Categories.OrderBy(x => x.CategoryName).ToList()));
                    }
                    else    // Yoksa da nesneyi yine olduğu gibi ekliyoruz.
                    {
                        list2.Add(new SelectListItem()
                        {
                            Text = category.CategoryName,
                            Value = category.Id.ToString()
                        });
                    }

                }
                return list2;   // Bu metod içerisinde yani alt kategori ekleme işlemi bittikten sonra listeyi donduruyoruz. Böylece toplu ekleme işlemi gerçekleşmiş oluyoruz.
            }
            return list; // Tüm ekleme işlemleri bittikten sonra da en dışdaki metodun listesini donduruyoruz.
        }

        protected List<SelectListItem> GetProductSelectList()
        {
            var products = new ProductRepo()    // Üst ürünü olmayan ve product tipi perakende olan ürünleri getir
                .GetAll(x => x.SupProductId == null && x.ProductType == ProductTypes.Retail)
                .OrderBy(x => x.ProductName);

            var list = new List<SelectListItem>()   // Bir liste oluşturduk
            {
                new SelectListItem()    // Listenin ilk elemanı 
                {
                    Text="Perakende Ürünü Yok",
                    Value=new Guid().ToString()
                }
            };

            foreach (var product in products)
            {
                if(product.Products.Any(x=>x.ProductType == ProductTypes.Retail))   // ProductType ı Retail olanların her birini listeye ekledik
                {
                    list.Add(new SelectListItem()
                    {
                        Text=product.ProductName,
                        Value=product.Id.ToString()
                    });
                    list.AddRange(GetSubProducts(product.Products.Where(x => x.ProductType == ProductTypes.Retail).OrderBy(x => x.ProductName).ToList()));  // Ve bunlar arasında alt ürünleri perakende olanları da bulup GetSubProducts metoduna parametre olarak verdik.
                }
                else
                {
                    list.Add(new SelectListItem()   // ProductType ı Retail olmayanların her birini de listeye ekledik
                    {
                        Text = product.ProductName,
                        Value = product.Id.ToString()
                    });
                }
            }

            List<SelectListItem> GetSubProducts(List<Product> products2)    // Yukarıdaki işlemlerin aynısını bu metod için de yaptık. Kendisini tekrar çağıran metod kullandık.
            {
                var list2 = new List<SelectListItem>();
                foreach (var product in products2)
                {
                    if (product.Products.Any(x => x.ProductType == ProductTypes.Retail))
                    {
                        list2.Add(new SelectListItem()
                        {
                            Text = product.ProductName,
                            Value = product.Id.ToString()
                        });
                        list2.AddRange(GetSubProducts(product.Products.Where(x => x.ProductType == ProductTypes.Retail).OrderBy(x => x.ProductName).ToList()));
                    }
                    else
                    {
                        list2.Add(new SelectListItem()
                        {
                            Text = product.ProductName,
                            Value = product.Id.ToString()
                        });
                    }
                }
                return list2;
            }

            return list;
        }

    }
}