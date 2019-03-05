﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Admin.BLL.Helpers
{
    public static class StringHelpers
    {
        public static string UrlFormatConverter(string name)
        {
            // Url de sıkıntı çıkaracak karakterlerde donuşum yapan metod
            string sonuc = name.ToLower();
            sonuc = sonuc.Replace("'", "");
            sonuc = sonuc.Replace(" ", "-");
            sonuc = sonuc.Replace("<", "");
            sonuc = sonuc.Replace(">", "");
            sonuc = sonuc.Replace("&", "");
            sonuc = sonuc.Replace("[", "");
            sonuc = sonuc.Replace("!", "");
            sonuc = sonuc.Replace("]", "");
            sonuc = sonuc.Replace("ı", "i");
            sonuc = sonuc.Replace("ö", "o");
            sonuc = sonuc.Replace("ü", "u");
            sonuc = sonuc.Replace("ş", "s");
            sonuc = sonuc.Replace("ç", "c");
            sonuc = sonuc.Replace("ğ", "g");
            sonuc = sonuc.Replace("İ", "I");
            sonuc = sonuc.Replace("Ö", "O");
            sonuc = sonuc.Replace("Ü", "U");
            sonuc = sonuc.Replace("Ş", "S");
            sonuc = sonuc.Replace("Ç", "C");
            sonuc = sonuc.Replace("Ğ", "G");
            sonuc = sonuc.Replace("|", "");
            sonuc = sonuc.Replace(".", "-");
            sonuc = sonuc.Replace("?", "-");
            sonuc = sonuc.Replace(";", "-");
            sonuc = sonuc.Replace("#", "-sharp");

            return sonuc;
        }

        public static string Capitilize(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            var items = text.Split(' ');
            var result = string.Empty;
            foreach (var item in items)
            {
                if (item.Length > 1)
                    result += $"{(item.Substring(0, 1).ToUpper())}{item.Substring(1).ToLower()} ";
                else
                    result += $"{item} ";
            }
            return result.Trim();
        }

        // Benzersiz kod üreten metod
        public static string GetCode() => Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "").ToLower(new CultureInfo("en-US", false));    // CultureInfo ile o kültüre göre donuşum yapıyor. en : İlk yazılan dil ve küçük harfle yazılır, US: ikinci yazılan bölge ve buyuk harfle yazılır.(tr-TR)
    }
}
