using Admin.BLL.Identity;
using Admin.Models.IdentityModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
// WebApi için bu kodlar sonradan eklendi
using System.Web.Http;
using System.Web.Routing;

namespace Admin.Web.UI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);   // WebApi için bu kod sonradan eklendi
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            // Role tablosunun içerisine Admin ve User adında iki insert yapılmasına yardımcı kodlar.Uygulama çalışmaya ilk buradan basladıgı için sayfa açıldıgında iki tane role olmuş oluyor.
            var roller =new string[] { "Admin", "User" };
            var roleManager=MembershipTools.NewRoleManager();
            foreach (var rol in roller)
            {
                if (!roleManager.RoleExists(rol))   // Role un olup olmadıgının kontrolu burada yapılıyor.
                {
                    roleManager.Create(new Role()   // Yeni bir Role ekleme işlemi burada gerçekleşiyor.
                    {
                        Name=rol
                    });
                }
            }
        }
    }
}
