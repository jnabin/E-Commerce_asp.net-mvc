using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Commerce.Models
{
    public class ProductMetaData
    {
        public int Product_id { get; set; }
        [Required]
        public string Product_name { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryID { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Range(1, 99999.99, ErrorMessage = "invalid amount of price")]
        public decimal UnitPrice { get; set; }
      
        public string ImageFile { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Range(1, 99999.99, ErrorMessage = "invalid amount of cost")]
        public decimal Cost { get; set; }
        [Required]
        public string SizeCategory { get; set; }
    }
}