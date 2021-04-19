using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Commerce.Models
{
    public class AdminMetaData
    {
        public int AdminID { get; set; }
        [Required(ErrorMessage = "Customer name can't be empty"), MaxLength(15, ErrorMessage = "Max length is 15")]
        public string AdminName { get; set; }
        [Required]
        [RegularExpression("^(?:\\+8801)?\\d{9}$", ErrorMessage = "Provide valid phone number with +880")]
        public string PhoneNum { get; set; }
        public string ImageFile { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [StringLength(20, MinimumLength = 8)]
        public string Address { get; set; }
        public string DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 5)]
        [RegularExpression("^([a-zA-Z])[a-zA-Z_-]*[\\w_-]*[\\S]$|^([a-zA-Z])[0-9_-]*[\\S]$|^[a-zA-Z]*[\\S]$", ErrorMessage = "username start with alphabets")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, MinimumLength = 4)]
        public string Password { get; set; }
        public string Usertype { get; set; }
    }
}