using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.ViewModels
{
    // Bir sayfaya birden fazla model gondermek istediğimizde MultipleViewModel kullanıyoruz. Bunun bir örnegi:
    public class RegisterLoginViewModel
    {
        // İki modelin birden kullanımını saglıyor
        public LoginViewModel LoginViewModel { get; set; }
        public RegisterViewModel RegisterViewModel { get; set; }
    }
}
