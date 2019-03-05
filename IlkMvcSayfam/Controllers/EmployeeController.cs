using IlkMvcSayfam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IlkMvcSayfam.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            var data = new NorthwindSabahEntities()
                .Employees
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();
            return View(data);
        }
        public ActionResult Detail(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var data = new NorthwindSabahEntities().Employees.Find(id.Value);
            if (data == null) RedirectToAction("Index");

            return View(data);
        }
        [HttpPost]
        public ActionResult Add(Employee employees)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                db.Employees.Add(employees);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Update(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            try
            {
                var data = new NorthwindSabahEntities().Employees.Find(id.Value);
                if (data == null)
                    return RedirectToAction("Index");
                return View(data);
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult Update(Employee model)
        {
            try
            {
                var db = new NorthwindSabahEntities();
                var data = db.Employees.Find(model.EmployeeID);

                if (data == null) return RedirectToAction("Index");
                data.FirstName = model.FirstName;
                data.LastName = model.LastName;
                data.HomePhone = model.HomePhone;
                data.Address = model.Address;
                db.SaveChanges();
                ViewBag.Message = "<span class='text text-success'>Update Successfuly</span>";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"<span class='text text-danger'>Update Error {ex.Message}</span>";
                return View(model);
            }
        }
        public ActionResult Delete(int? id)
        {
            var db = new NorthwindSabahEntities();
            try
            {
                var data = db.Employees.Find(id.GetValueOrDefault());
                if (data == null) return RedirectToAction("Index");

                db.Employees.Remove(data);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
    }
}