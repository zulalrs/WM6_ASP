using AutoMapper;
using Kuzey.Models.Entities;
using Kuzey.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kuzey.Web.App_Start
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg=>
            {
                CategoryMapping(cfg);
                ProductMapping(cfg);
            });
        }

        private static void ProductMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Product, ProductViewModel>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(x => x.Category.CategoryName)) // Classlar arasında farklılık oldugu zaman yani ekstradan property yazıldıgı zamanlarda bu satırı kullanıyoruz ve navigation propertyler üzerinden eşleşme saglamaya çalışıyroruz. İki parametre alıyor(destination ve option(işlem)) // Ürünler tablosundaki categoryName ,Category tablosundaki CategoryNameden gelsin. Yani o ikisi arasında eşleşme yap anlamında.
                .ReverseMap();
        }

        private static void CategoryMapping(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Category, CategoryViewModel>()    // Category ile CategoryViewModel i maple anlamında
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(x => x.Products.Count))
                .ReverseMap();  // Tam tersi şekilde de maplemesi için kullanılıyor.
        }
    }
}