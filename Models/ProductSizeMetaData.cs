using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Commerce.Models
{
    public class ProductSizeMetaData
    {
        
        public int ProductSizeID { get; set; }
        [Required]
        public int ProductID { get; set; }
        [Required]
        [RegularExpression("^[1-9]\\d*$", ErrorMessage ="invalid quantity")]
        [Range(1,100, ErrorMessage ="quantity should be between 1 to 100")]
        public int Count { get; set; }
        [Required]
        public string SizeName { get; set; }
    }
}