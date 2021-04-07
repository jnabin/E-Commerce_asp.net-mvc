using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Models.ViewModels
{
    public class SizeViewModel
    {
        public ProductSize productSizes { get; set; }
        public List<string> sizelist { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public Product product { get; set; }
    }
}