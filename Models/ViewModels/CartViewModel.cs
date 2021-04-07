using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Commerce.Models.ViewModels
{
    public class CartViewModel
    {
        public string name { get; set; }
        public Product product { get; set; }
        public int count { get; set; }
        public ProductSize size { get; set; }
        public string outOfStock { get; set; }
    }
}