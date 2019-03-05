using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.IdentityModels
{
    // IdentityUser: User a ait bir takım propertylerin bulundugu sınıf. Bizim yazdığımız sınıfın bu sınıftan kalıtım almasını sağlayarak oradaki bilgilere de ulaşmış olduk. IdentityUser daki bilgilere ekleme yapmak istediğimiz için böyle bir sınıf oluşturduk. Böylece Identityuser tablosunu genişletmiş olduk.IdentityUser ın içindeki prop lar bize yeterli olacaksa böyle bir sınıf oluşturmasakta olur.
    public class User: IdentityUser
    {
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        [StringLength(60)]
        [Required]
        public string Surname { get; set; }
        public string  ActivationCode { get; set; }

    }
}
