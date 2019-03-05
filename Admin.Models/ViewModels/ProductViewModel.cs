using Admin.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Admin.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public string AvatarPath { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }
    }
}
