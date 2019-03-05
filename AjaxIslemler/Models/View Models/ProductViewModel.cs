using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjaxIslemler.Models.View_Models
{
    //Döngüsel donuşum hatasını engellemek adına view model oluşturduk.
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public string UnitPriceFormatted { get; set; }  // Javascriptte UnitPrice i formatlayamayacagımız için yani string formatı kullanamayacagımız için böyle bir property oluşturduk.
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedDateFormatted { get; set; }  // Javascriptte addeddate i formatlayamayacagımız için yani string formatı kullanamayacagımız için böyle bir property oluşturduk.
        public string CategoryName { get; set; }
        public string SupplierName { get; set; }
    }
}