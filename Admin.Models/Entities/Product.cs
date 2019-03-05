using Admin.Models.Abstracts;
using Admin.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.Entities
{
    [Table("Products")]
    public class Product:BaseEntity<Guid>
    {
        public Product()
        {
            this.Id = Guid.NewGuid();
        }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "Ürün adı 1 ile 100 karakter aralığında olmalıdır")]
        [Required]
        [DisplayName("Ürün Adı")]
        public string ProductName { get; set; }


        [DisplayName("Ürün Tipi")]
        public ProductTypes ProductType { get; set; }

        [DisplayName("Satış Fiyatı")]
        public decimal SalesPrice { get; set; }

        [DisplayName("Alış Fiyatı")]
        public decimal BuyPrice { get; set; }

        [DisplayName("Stok Miktarı")]
        [Range(0, 9999)]
        public double UnitsInStock { get; set; }

        [DisplayName("Fiyat Güncellenme Tarihi")]
        public DateTime LastPriceUpdateDate { get; set; }

        [DisplayName("Kategorisi")]
        public int CategoryId { get; set; } // Category ile bu column üzerinden eşleşecekler

        [DisplayName("Perakende Ürünü")]
        public Guid? SupProductId { get; set; } // Toptan ürünler ile bu column üzerinden eşleşecekler. Toptan ürünün bir üstü olmayacagı için toptan ürüler için bu ıd null olmalı.

        [StringLength(20)]
        [Required]
        [Index(IsUnique = true)]    // Benzersiz ve nonclustred olacak. Nonclustred ifadeleri ararken daha kolay buluruz ve arama daha hızlı sonuclanır.
        public string Barcode { get; set; }

        [DisplayName("Birim")]
        public int Quantity { get; set; }

        [DisplayName("Açıklama")]
        public string Description { get; set; }
        public string AvatarPath { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }  // Bir ürün bir kategori altında olabilir.
        [ForeignKey("SupProductId")]
        public virtual Product SupProduct { get; set; } // Bir ürünün bir toptan ı yani üst ürünü olabilir.
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();    // Bir ürünün birden fazla alt ürünü olabilir.
        public virtual ICollection<Invoice> Invoices { get; set; } = new HashSet<Invoice>();    // Bir faturada birden fazla ürün olabilir.
    }
}
