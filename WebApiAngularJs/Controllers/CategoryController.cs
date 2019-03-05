using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiAngularJs.Models;

namespace WebApiAngularJs.Controllers
{
    public class CategoryController : ApiController
    {
        MyCon db = new MyCon();
        // 1) Tüm listeyi getirme işlemi
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            try
            {
                return Ok(new   // Yeni bir nesne oluşturup onu donduruyoruz. Normalde böyle bir nesnemiz yok burada new diyerek oluşturduk.
                {
                    success = true,
                    data = db.Categories.Select(x => new CategoryViewModel()
                    {
                        CategoryID = x.CategoryID,
                        CategoryName = x.CategoryName,
                        Description = x.Description
                    }).ToList() // data olarak CategoryViewModel tipinde bir nesne dondurecek.
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}"); // Bir hata oldugunda da buradaki mesajı dondurecek.
            }
        }

        // 2) İstenilen id deki veriyi getirme
        [HttpGet]
        public IHttpActionResult Get(int id=0)
        {
            try
            {
                var cat = db.Categories.Find(id);
                if (cat == null)
                    return NotFound();  // Eger gelen id de category yoksa notfound metodunu dondur.

                var data = new CategoryViewModel() // Varsa da yeni bir CategoryViewModel nesnesi oluştur ve gelen nesneye göre propertyleri doldur.
                {
                    CategoryID=cat.CategoryID,
                    CategoryName=cat.CategoryName,
                    Description=cat.Description
                };
                return Ok(new   // Ve hata olmazsa datayı döndür.
                {
                    success=true,
                    data=data
                });
            }
            catch (Exception ex)    // Hata olduğunda
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }
        }

        // 3) Kategori ekleme işlemi
        [HttpPost]
        public IHttpActionResult Add([FromBody]CategoryViewModel model)
        {
            try
            {
                db.Categories.Add(new Category() {
                    CategoryName=model.CategoryName,
                    Description=model.Description
                });
                db.SaveChanges();
                return Ok(new {
                    success=true,
                    message="Kategori ekleme işlemi başarılı"
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }
        }

        // 4) Kategori silme işlemi
        [HttpDelete]
        public IHttpActionResult Delete(int id=0)
        {
            try
            {
                db.Categories.Remove(db.Categories.Find(id));
                db.SaveChanges();
                return Ok(new {
                    success=true,
                    message="Kategori silme işlemi başarılı."
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Bir hata oluştu {ex.Message}");
            }
        }

        // 5) Guncelleme işlemi
        [HttpPut]
        public IHttpActionResult PutCategory(int id, Category model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != model.CategoryID)
            {
                return BadRequest();
            }

            db.Entry(model).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                return Ok(new
                {
                    success = true,
                    message = "Kategori Güncelleme işlemi başarılı"
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!db.Categories.Any(x => x.CategoryID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}
