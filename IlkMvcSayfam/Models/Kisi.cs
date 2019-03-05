using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IlkMvcSayfam.Models
{
    public class Kisi
    {
        // Kisi tipinde bir model taımladık.
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}