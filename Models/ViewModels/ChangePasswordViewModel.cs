using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Commerce.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int CustomerID { get; set; }

        public string password { get; set; }

        public string newpassword { get; set; }

        public string confirmpassword { get; set; }

        public string originalpass { get; set; }
    }
}