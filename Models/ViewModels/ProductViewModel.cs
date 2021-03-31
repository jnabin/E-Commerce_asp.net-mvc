using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Commerce.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Product_id { get; set; }
        public string Product_name { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Onhand { get; set; }
        public string ImageFile { get; set; }
        public List<string> sizename { get; set; }
        public List<int> sizeid { get; set; }
        public List<int> sizecount { get; set; }
        public Nullable<int> FinalSubCategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public string SizeCategory { get; set; }
       
    }
}