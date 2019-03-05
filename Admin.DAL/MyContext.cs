using Admin.Models.Entities;
using Admin.Models.IdentityModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.DAL
{
    public class MyContext:IdentityDbContext<User>  // Generic olmayını kullandığımızda standart IdentityUser ve IdentityRole tablolarını kullanır ama Generic olanı kullanacaksakta hangi kullanıcı üzerinden çalışacagımızı belirtiyoruz. Bizim kullanacagımız ise User sınıfı.
    {
        public MyContext():base("name=MyCon")
        {

        }
        public DateTime InstanceDate { get; set; }  // Instance ın ne zaman alındıgını tutuyor.
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
                .Property(x => x.TaxRate)
                .HasPrecision(4, 2);    //Toplamda 4 basamak virgulden sonra 2 basamak anlamında

            modelBuilder.Entity<Product>()
                .Property(x => x.BuyPrice)
                .HasPrecision(7, 2); // Toplamda 7 basamaklı virgulden sonra 2 virgulden önce 3 basamak anlamında
            modelBuilder.Entity<Product>()
                .Property(x => x.SalesPrice)
                .HasPrecision(7, 2);

            modelBuilder.Entity<Invoice>()
               .Property(x => x.Quantity)
               .HasPrecision(8, 4);

            modelBuilder.Entity<Invoice>()
                .Property(x => x.Price)
                .HasPrecision(9, 3);

            modelBuilder.Entity<Invoice>()
                .Property(x => x.Discount)
                .HasPrecision(3, 2);
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
    }
}
