using Admin.BLL.Identity;
using Admin.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Admin.Web.UI.Controllers.WebApi
{
    public class AccountController : ApiController
    {
        [Authorize] // Buraya Authorize yazmamız önemli
        // Giriş yapan kullanıcının bilgilerini aldık.
        public IHttpActionResult GetLoginInfo()
        {
            var userManager = MembershipTools.NewUserManager();
            var user = userManager.FindById(HttpContext.Current.User.Identity.GetUserId());
            return Ok(new UserProfileViewModel() {
                Name=user.Name,
                UserName=user.UserName,
                Email=user.Email,
                Surname=user.Surname,
                Id=user.Id,
                //AvatarPath = user.AvatarPath
            });
        }
    }
}
