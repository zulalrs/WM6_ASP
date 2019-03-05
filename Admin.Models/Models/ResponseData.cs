using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models.Models
{
    // JsonResult işlemleri için kullandığımız sınıf
    public class ResponseData
    {
        public string message { get; set; }
        public bool success { get; set; }
        public object data { get; set; }
        public DateTime responseTime { get; set; } = DateTime.Now;
        public string responseTimeU { get; set; } = $"{DateTime.Now:O}";
    }
}
