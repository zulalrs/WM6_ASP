using Admin.DAL;
using Admin.Models.IdentityModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Admin.BLL.Identity
{
    public static class MembershipTools
    {
        private static MyContext _db;

        public static UserStore<User> NewUserStore() => new UserStore<User>(_db ?? new MyContext());    // UserStore ihtiyacımız olduğunda NewUserStore metodunu kullanacagız. Eger daha once bir instance alınmışsa o zaman onu kullanıyor, alınmamışsa da yeni bir instance alıyor.
        public static UserManager<User> NewUserManager() => new UserManager<User>(NewUserStore());  // UserManager ihtiyacımız olduğunda NewUserManager metodunu kullanacagız.

        public static RoleStore<Role> NewRoleStore() => new RoleStore<Role>(_db ?? new MyContext());
        public static RoleManager<Role> NewRoleManager() => new RoleManager<Role>(NewRoleStore());

        // Home Index sayfasında giriş yapan kullanıcının adını ve soyadını görmek için kullanacagımmız metod:
        public static string GetNameSurname(string userId)  // Eger null gelirse o an kim login olduysa onun ad soyadını yazacak.Eger birisi parametre gonderdiyse de o id ye göre ad soyad yazacak
        {
            User user;
            if(string.IsNullOrEmpty(userId))    // Burada mevcut kullanıcının bilgisini döndürecek.
            {
                var id=HttpContext.Current.User.Identity.GetUserId();   // Giriş yapmış olan kullanıcının id sini aldık.
                if(string.IsNullOrEmpty(id))    // Giriş yapmış bir kullanıcı yoksa ve bu metodu çağırmış isek
                {
                    return ""; // Boş bir string donsun
                }

                user=NewUserManager().FindById(id); // Bu id deki kullanıcıyı getir.
            }
            else    // parametre olan userId gelmiş ise:
            {
                user = NewUserManager().FindById(userId);   // Bu id li kullanıcıyı getir.
                if (user == null)   // Eger parametreye yanlış bir id göndermiş iseler null dönecek. Yani index sayfasındaki idi alanı boş olacak.
                    return null;
            }
            return $"{user.Name} {user.Surname}";   //Kullanıcı bulundugu takdirde isim ve soyismi dondurulecek
        }
    }
}
