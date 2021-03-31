using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Commerce.Models.ViewModels
{
    public class CategoryViewModel
    {
        public List<MainCategory> mainCategories = new List<MainCategory>();
        public List<SubCatetory> subCatetories = new List<SubCatetory>();
        public List<FinalSubCategory> finalSubCategories = new List<FinalSubCategory>();
    }
}