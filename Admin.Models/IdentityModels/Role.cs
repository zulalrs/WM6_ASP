using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.IdentityModels
{
    // IdentityUser: User a ait bir takım propertylerin bulundugu sınıf. Bizim yazdığımız sınıfın bu sınıftan kalıtım almasını sağlayarak oradaki bilgilere de ulaşmış olduk.
    public class Role:IdentityRole
    {
        public Role()
        {

        }
        public Role(string description)
        {
            Description = description;
        }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
