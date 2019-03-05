using Admin.Models.Abstracts;
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
    [Table("Categories")]
    public class Category:BaseEntity<int>
    {
        [StringLength(100,ErrorMessage ="Kategori adı 3 ile 100 karakter arasında olabilir.",MinimumLength =3)]
        [DisplayName("Kategori Adı")]   // Sayfada gözükecek isim
        [Required]
        public string CategoryName { get; set; }

        [Range(0,99)]
        [DisplayName("KDV Oranı")]
        public decimal TaxRate { get; set; }

        [DisplayName("Üst Kategori")]
        public int? SupCategoryId { get; set; } // üst kategori null olabilir. Null ise zaten kategori en üsttedir.  İlişki bu column ile yapılacak





        [ForeignKey("SupCategoryId")]
        // Self join start
        public virtual Category SupCategory { get; set; }   // Her kategorinin bir üst kategorisi vardır.
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();    // Her kategorisnin birden fazla alt kategorisi vardır.
        // Self join end
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();    // Bir kategoride birden fazla urun olabilir
    }
}
