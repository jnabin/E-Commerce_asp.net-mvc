using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Commerce.Models
{
    public class CustomerMetaData
    {
        public int CustomerID { get; set; }
        [Required(ErrorMessage ="Customer name can't be empty"), MaxLength(15, ErrorMessage ="Max length is 15")]
        public string CustomerName { get; set; }
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(15, MinimumLength = 8)]
        
        public string Address { get; set; }
        [Required]
        [StringLength(15, MinimumLength =4)]
        [DataType(DataType.Text)]
        public string City { get; set; }
        [Required]
        [Range(1000, 9999, ErrorMessage ="zipcode should be 4 characters with all numbers")]
        public string ZipCode { get; set; }
        [Required]  
        [RegularExpression("^(?:\\+8801)?\\d{9}$", ErrorMessage ="Provide valid phone number with +880")]
        public string PhoneNumber { get; set; }
        [Required]
        
        [DataType(DataType.ImageUrl)]
        public string ImageFile { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 5)]
        [RegularExpression("^([a-zA-Z])[a-zA-Z_-]*[\\w_-]*[\\S]$|^([a-zA-Z])[0-9_-]*[\\S]$|^[a-zA-Z]*[\\S]$", ErrorMessage ="username start with alphabets")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 4)]
        public string password { get; set; }
        public string usertype { get; set; }
    }
}