using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Commerce.Models.ViewModels
{
    public class SortViewModel
    {
        public int Product_id { get; set; }
        public string Product_name { get; set; } 
        public decimal UnitPrice { get; set; }
        public string ImageFile { get; set; }
        public int totalorder { get; set; }
        public int totalreview { get; set; }

    }
}