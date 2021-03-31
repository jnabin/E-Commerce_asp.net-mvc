using E_Commerce.Models.ViewModels;
using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class CategoryController : Controller
    {
        
        [ChildActionOnly]
        public ActionResult NavBar()
        {
            MainCategoryRepository mainCategoryRepository = new MainCategoryRepository();
            SubCategoryRepository subCategoryRepository = new SubCategoryRepository();
            FinalSubCategoryRepository finalSubCategoryRepository = new FinalSubCategoryRepository();
            CategoryViewModel categoryViewModel = new CategoryViewModel();
            categoryViewModel.mainCategories = mainCategoryRepository.GetAll();
            categoryViewModel.subCatetories = subCategoryRepository.GetAll();
            categoryViewModel.finalSubCategories = finalSubCategoryRepository.GetAll();
            return PartialView("_NavBar", categoryViewModel);
        }
    }
}