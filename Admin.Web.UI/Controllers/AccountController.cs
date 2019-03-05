using Admin.BLL.Helpers;
using Admin.BLL.Identity;
using Admin.BLL.Services.Senders;
using Admin.Models.IdentityModels;
using Admin.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
// Yeni özellik ile buraya static nesnelerimizi ekleyebiliyoruz.
using static Admin.BLL.Identity.MembershipTools;

namespace Admin.Web.UI.Controllers
{

    public class AccountController : Controller
    {
        // GET: Account
        //[Authorize(Roles ="Admin")] Sadece admin rolunde olanlar bu sayfayı görür anlamında yani sayfayı görebilmesi için login olması yetmeyecek bir de admin rolunde olması gerekecek
        public ActionResult Index()
        {
            if (HttpContext.GetOwinContext().Authentication.User.Identity.IsAuthenticated)  // Account index açıldığında bir kullanıcı giriş yapmış yani varsa bizi direk Home index e yonlendirecek
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Kayıt bilgilerinin gönderilmesi için post kullandık.
        // RegisterLoginViewModel kullanmamızın sebebi de indexte o model tipini kullanıyor olmamız. Verilerin girilmesi ile birlikte bize Indexten bir model gelecek
        public async Task<ActionResult> Register(RegisterLoginViewModel model)
        {
            if (!ModelState.IsValid) // Eger Indexten gelen ve gonderilecek olan modelimiz  Isvalid değilse o zaman ındex viewdan gelen modeli oldugu gibi ındex e geri gönderiyoruz.
            {
                return View("Index", model);
            }
            try
            {
                // User ile ilgili işlemler yapacaksak bize UserManeger gerekecek. Onun için tanımlama yapıyoruz.
                var userManager = NewUserManager();
                // Veritabanı işlemi yapabilirz diye de store tanımlaması yaptık
                var userStore = NewUserStore();
                // Önce kullanıcının olup olmadıgının kontrolunu yapıyoruz. 
                // model.RegisterViewModel.UserName e göre kullanıcılarımız arasında arama yani kontrol yapmalı.
                var rm = model.RegisterViewModel;   // Çok uzun olmasın diye burada bir bölme işlemi yaptık.
                var user = await userManager.FindByNameAsync(rm.UserName);  // UserManager ile bu modelin olup olmadıgını kontrol edecek
                if (user != null)  // Eger null degilse böyle bir kullanıcı var demektir. Bunun için modelstate imizin içerisine username için bir hata mesajı yazdık.
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı daha önceden alınmıştır.");   // Hata mesajımız
                    return View("Index", model);    // Ve index viewına modelimizi geri donduruyoruz. Yani yine aynı sayfa yazdığımız verilerle beraber geliyor.
                }
                // Tüm aşamaları geçip buraya geldiğinde yeni bir user oluşturuyoruz.
                var newUser = new User()
                {
                    UserName = rm.UserName,
                    Email = rm.Email,
                    Name = rm.Name,
                    Surname = rm.Surname,
                    ActivationCode = StringHelpers.GetCode()  // Her yeni bir kullanıcı oluştuğunda activasyon kodu da oluşsun
                };
                var result = await userManager.CreateAsync(newUser, rm.Password);  // Password u ayrı istiyor çükü onu şifreleyecek
                if (result.Succeeded)    // Eger kayıt başarılı ise admin mi yoksa user mı olacagına karar vermeliyiz. Eger ilk kullanıcı ise onu admin yapar, diğer kulanıcılar için de user role unu verebiliriz. Bunun için ilk kullanıcı olup olmadıgının kontrolunu yapacagız.
                {
                    if (userStore.Users.Count() == 1)  // 1 e eşit olursa bu ilk ve tek kayıt oldugunu gösterir. Yani ilk kayıt oldugu için bunu admin yapabiliriz.
                    {
                        await userManager.AddToRoleAsync(newUser.Id, "Admin");   // Bu user ı Admin olarak eklemiş olduk.  
                    }
                    else    // Şart saglanmıyorsa da bu kullanıcıyı user olarak ekleyecek.
                    {
                        await userManager.AddToRoleAsync(newUser.Id, "User");
                    }
                    // ***Mail gonderme işlemi
                    string SiteUrl = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
                                 (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port); // Farklı pc lerden çalıştırma sonucu doğacak url hatalarını önlüyor. Url yonetimini saglıyor. Sitenin çalıştığı mevcut yapıyı veriyor.

                    var emailService = new EmailService();  // Kendi oluşturdugumuz maili kullanacagız o maili de boş constructora yazdıgımız için bunu çağırdık. Eger dışarıdan vermek isteseydık o zaman parametreli constructorı newleyecektik.
                    // EmailService içindeki hiçbir propertyi kullanmayacagımız için burada onları çağırmadık, sadece send metodunu çağırdık.
                    var body = $"Merhaba <b>{newUser.Name} {newUser.Surname}</b><br>Hesabınızı aktif etmek için aşağıdaki linke tıklayınız.<br><a href='{SiteUrl}/account/activation?code={newUser.ActivationCode}'>Aktivasyon Linki</a>";  // body içerisine mesajı ve linki yazdık
                    await emailService.SendAsync(new IdentityMessage()
                    {
                        Body = body,
                        Subject = "Sitemize Hoşgeldiniz",
                    }, newUser.Email);   // EmailService parametrelerini buradan gonderdik.
                }
                else // Eger kayıt başarılı değilse donecek hata mesajları
                {
                    var err = "";
                    foreach (var resultError in result.Errors)
                    {
                        err += resultError + " ";   // Her bir hatayı err ye ekliyor ve hata oldugunda en son o gösterilecek.
                    }
                    ModelState.AddModelError("", err);   // Hata mesajımız
                    return View("Index", model);    // Ve tekrar aynı sayfa yazdığımız verilerle beraber gelecek.
                }
                TempData["Mesaj"] = "Kaydınız alınmıştır.Lütfen giriş yapınız"; // View kısmına bu mesajın görülmesini istediğimiz yere bunu yazıyoruz.
                return RedirectToAction("Index");   // En son olrakta Index sayfasını dondur.
            }
            catch (Exception ex)
            {
                // Burayı hata classını yazınca doldur.Video3 16:57
                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Login işlemleri
        public async Task<ActionResult> Login(RegisterLoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid) // Viewdan gelen modelim isvalid değilse yine Index ekranına girdiğimiz verilerle donduruluyoruz.
                {
                    return View("Index", model);
                }
                // User ile ilgili işlemler yapacagımız için burada da yine usermanager tanımladık.
                var userManager = NewUserManager();
                // Giriş için kullanıcının kayıtlı olup olmadıgını kontrol etmemiz gerekiyor.
                var user = await userManager.FindAsync(model.LoginViewModel.UserName, model.LoginViewModel.Password); // Gelen modele göre kullanıcıyı bulmaya çalışacak
                if (user == null)  //Eger kullanıcı veya şifre hatalıysa null gelecek. Null gelirse de:
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                    return View("Index", model);
                }
                // Bulduğumuz login ile giriş yapabilmemiz için bir tane Authentication manager a ihtiyacımız var. Aşagıdaki kod bize bu konuda yardımcı oluyor.Bu methodun içinde signin ve signout metodları var.Onları kullanacagız.
                var authManager = HttpContext.GetOwinContext().Authentication;
                // SignIn bizden Identity istiyor. Bunun için bir userIdentity oluşturuyoruz.
                // SignIn bizden Identity istiyor. Bunun için bir userIdentity oluşturuyoruz.
                var userIdentity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                authManager.SignIn(new AuthenticationProperties()
                {
                    // Once bizden bir ayar istiyor parametre olarak,sonra da identity i 
                    IsPersistent = model.LoginViewModel.RememberMe // True olursa beni hatırla özelliğini sectiğimizde siteyi kapatıp açsak bile loginimizin kalmasını saglıyor. Ama biz burada true false özelliğini modelimize bağladık, oradan alacak.
                }, userIdentity);
                // Login işlemi başarılı oldugunda bizi HomeControllerında tanımlı index sayfasına gonderecek.
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        // Herhangi bir veri godermeden direk olarak get ile çıkacagız
        public ActionResult Logout()
        {
            // Logout ile çıkış yapabilmemiz için bir tane Authentication manager a ihtiyacımız var.
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Index");
        }


        //** Profil guncelleme ve şifre değiştirme işlemleri aynı anda yapılmayacagı için iki ayrı view model oluşturacagız.Bizim temamızda bunlar aynı sayfada oldugu için de multiviewmodel ile bu iki modeli birleştireceğiz.
        [HttpGet]
        [Authorize]
        // Profil işlemleri
        public ActionResult UserProfile()
        {
            // Profile linkine tıkladığımızda sayfada profil bilgilerimizi gormemiz gerekir onun için burada doldurma işlemi yapıyoruz.
            try
            {
                var id = HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId(); // Baglı olan kullanıcının id sini dondurecek.
                var user = NewUserManager().FindById(id); // Bu id deki kullanıcıyı bulduk.
                // Kullanıcıyı bulduktan sonra returnda kullanacagımız modeli tanımlamamız gerekecek.
                // Object initialize ile doldurma işlemi yaptık.
                var data = new ProfilePasswordViewModel()
                {
                    UserProfileViewModel = new UserProfileViewModel()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        PhoneNumber = user.PhoneNumber,
                        UserName = user.UserName
                    }
                };
                return View(data);  // Datayı doldurduktan sonra view e gonderiyoruz. Böylece bilgilerimizin dolu gelmesini saglamış olduk.
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        // UserProfile viewden guncelleye tıkladığımıza bize oradan ProfilePasswordViewModel tipinde bir model gelecek onu parametre olarak alacagız.
        public async Task<ActionResult> UpdateProfile(ProfilePasswordViewModel model)
        {
            if (!ModelState.IsValid) // Modelimiz gecerli değilse UserProfile sayfasına geri donecegiz.
            {
                return View("UserProfile", model);
            }
            try
            {
                // Kullanıcıya ihtiyacımız var onu bulmak içinde usermanegerı kullanıyoruz.
                var userManager = NewUserManager();
                var user = await userManager.FindByIdAsync(model
                    .UserProfileViewModel.Id);  // User ı bulduk ve aşağıda gelen modele göre userımıza gerekli atamaları yaptık.
                user.Name = model.UserProfileViewModel.Name;
                user.Surname = model.UserProfileViewModel.Surname;
                user.PhoneNumber = model.UserProfileViewModel.PhoneNumber;
                // Mailde değişiklik yaptığımızda yeni maile aktivasyon kodu göndermek ve role de değişiklik yapmak için  bu if i yazdık.
                if (user.Email != model.UserProfileViewModel.Email)
                {
                    // todo tekrar aktıvasyon maili gonderilmeli rolu de aktif olmamış role cevrilmeli
                }
                user.Email = model.UserProfileViewModel.Email;

                await userManager.UpdateAsync(user);
                TempData["Message"] = "Guncelleme şlemi başarılı";
                return RedirectToAction("UserProfile");
            }
            catch (Exception)
            {
                // todo hata mesajlarını al
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        // UserProfile viewden şifre guncelleye tıkladığımıza bize oradan ProfilePasswordViewModel tipinde bir model gelecek onu parametre olarak alacagız.
        public async Task<ActionResult> ChangePassword(ProfilePasswordViewModel model)
        {

            try
            {
                // user ile ilgili işlemlerimiz oldugu için usermanager kullandık.
                // Model boş gelmesin diye burada modeli dolduruyoruz.
                var userManager = NewUserManager();
                var id = HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId();
                var user = NewUserManager().FindById(id);
                var data = new ProfilePasswordViewModel()
                {
                    UserProfileViewModel = new UserProfileViewModel()
                    {
                        Email = user.Email,
                        Id = user.Id,
                        Name = user.Name,
                        PhoneNumber = user.PhoneNumber,
                        Surname = user.Surname,
                        UserName = user.UserName
                    }
                };
                model.UserProfileViewModel = data.UserProfileViewModel; // Datayı modele eşitliyoruz böylece modelimiz artık giriş yapmış kişinin bilgilerini taşıyor.
                if (!ModelState.IsValid) // Modelimiz gecerli değilse UserProfile sayfasına geri donecegiz. 
                {
                    model.ChangePasswordViewModel = new ChangePasswordViewModel();  // Password alanının temizlenmesi için 
                    return View("UserProfile", model);
                }


                var result = await userManager.ChangePasswordAsync(
                    HttpContext.GetOwinContext().Authentication.User.Identity.GetUserId(),
                    model.ChangePasswordViewModel.OldPassword, model.ChangePasswordViewModel.NewPassword);// Giriş yapmış kullanıcının id sini getirdik ve usermangerdaki changepassword metodunu kullandık.

                if (result.Succeeded)    // Eger başarılı bir guncelleme yapıldıysa
                {
                    //todo kullanıcıyı bilgilendiren bir mail atılır
                    return RedirectToAction("Logout", "Account");
                }
                else // Hata varsa
                {
                    var err = "";
                    foreach (var resultError in result.Errors)
                    {
                        err += resultError + " ";
                    }
                    ModelState.AddModelError("", err);
                    model.ChangePasswordViewModel = new ChangePasswordViewModel();
                    return View("UserProfile", model);
                }
            }
            catch (Exception)
            {
                // todo hata mesajlarını ekle
                throw;
            }
        }



        [HttpGet]
        [AllowAnonymous]
        public ActionResult Activation(string code)
        {
            // Kullanıcıyı oluştururken activasyon kodu da oluşturmuştuk.Kullanıcıyı onunla bulabilliriz.
            try
            {
                var userStore = NewUserStore();
                var user = userStore.Users.FirstOrDefault(x => x.ActivationCode == code);  // aktivasyon koduna göre user arama
                if (user != null)
                {
                    if (user.EmailConfirmed) // Aktif ise tekrar aktive edilmesini engelleme
                    {
                        ViewBag.Message = $"<span class='text-green'>Bu hesap daha önce aktive edilmiştir</span>";
                    }
                    else
                    {
                        user.EmailConfirmed = true; // aktıvasyon gerçekleşirse true yapacak
                        userStore.Context.SaveChanges();    // user da değişiklik yapma         
                        ViewBag.Message = $"<span class='text-green'>Aktivasyon işleminiz başarılı</span>";
                    }

                }
                else
                {
                    ViewBag.Message = $"<span class='text-red'>Aktivasyon başarısız</span>";
                }
            }
            catch (Exception)
            {
                ViewBag.Message = $"<span class='text-red'>Aktivasyon işleminde bir hata oluştu</span>";
            }
            return View();
        }


        [HttpGet]
        [AllowAnonymous]    // Herkes ulaşabilir
        // şifremi unuttum kısmı
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        [AllowAnonymous]
        // şifremi unuttum kısmı
        public async Task<ActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            // İlk olarak kullanıcının email adresinden kullanıcıyı bulmamız lazım.Bunun için user maneger a ihtiyacımız var
            try
            {
                //userStore ve userManager ile bulduğumuz kullanıcı refaransları farklı olabilir onun için şifre değiştiriken userstore kullandığımız için maili bulurken de userstore kulandık.
                var userStore = NewUserStore();
                var userManager = NewUserManager();
                var user = await userStore.FindByEmailAsync(model.Email); // Kullanıcı arama işlemi
                if(user==null)  // Kullanıcı bulunamadığında gerçekleşecek işlemler:
                {
                    ModelState.AddModelError(string.Empty, $"{user.Email} mail adresine kayıtlı bir uyeliğe erişilemedi");
                    return View(model);
                }
                else    // Kullanıcı bulunduğunde gerçekleşecek işlemler:
                {
                    // Once bu kulanıcı için bir şifre uretmeliyiz ve sonrasında o şifreyi kullanıcının yeni şifresi olarak tanımlamalıyız.
                  
                    var newPassword = StringHelpers.GetCode().Substring(0, 6);  // Helper yardımıyla 6 haneli bir şifre oluşturduk.
                  await userStore.SetPasswordHashAsync(user, userManager.PasswordHasher.HashPassword(newPassword)); //SetPasswordHashAsync metodu bir kullanıcı ve hashlenmiş bir şifre istiyor. user maneger içindeki PasswordHasher nesnesi hashpassword metodu ile parametre olarak verdiğimiz yeni şifreyi hashleyecek. HashPassword metodu yeni şifreyi şifrelemiş olarak dondurecek.
                   var result= userStore.Context.SaveChanges(); // Değişiklikleri kaydederek kullanıcının şifresini değiştirmiş olduk.
                    if(result==0)
                    {
                        //TempData["Model"] = new ErrorViewModel()
                        //{
                        //    Text="Bir hata oluştu",
                        //    ActionName="RecoverPassword",
                        //    ControllerName="Account",
                        //    ErrorCode=500
                        //};
                        //return RedirectToAction("Error", "Hom");
                    }

                    // Yeni şifre için mail goderme işlemleri:
                    var emailService = new EmailService();
                    var body = $"Merhaba <b>{user.Name} {user.Surname}</b><br>Hesabınızın parolası sıfırlanmıştır.<br>Yeni parolanız:<b> {newPassword}</b><p>Yukarıdaki parolayı kullanarak sistemimize giriş yapabilirsiniz.</p>"; 
                    emailService.Send(new IdentityMessage()
                    {
                        Body = body,
                        Subject = $"{user.UserName} Şifre Kurtarma",
                    }, user.Email);
                }
            }
            catch (Exception)
            {
                // todo hata mesajını almayı unutma
                throw;
            }


            return View();
        }
    }
}