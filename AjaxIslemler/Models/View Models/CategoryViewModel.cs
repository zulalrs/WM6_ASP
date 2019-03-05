using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjaxIslemler.Models.View_Models
{
    public class CategoryViewModel
    {
        //Döngüsel donuşum hatasını engellemek adına view model oluşturduk.
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int ProductCount { get; set; }
    }
}