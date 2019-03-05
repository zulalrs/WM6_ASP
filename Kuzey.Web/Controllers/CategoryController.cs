using AutoMapper;
using Kuzey.BLL.Repository;
using Kuzey.Models.Entities;
using Kuzey.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kuzey.Web.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            var data = new CategoryRepo().GetAll().Select(x => Mapper.Map<CategoryViewModel>(x)).ToList();  // CategoryRepo dan tüm kategorileri getir ve viewmodele donustur
            return View(data);
        }

        [HttpPost]
        public ActionResult Add(CategoryViewModel model)
        {
            new CategoryRepo().Insert(Mapper.Map<CategoryViewModel, Category>(model));  // CategoryViewModel i Category e donuşturur ve oyle ınsert eder.Donuşturulecek nesneyi de parametre olarak vermeliyiz.
            return View();
        }
    }
}