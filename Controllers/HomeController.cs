using E_Commerce.Models.ViewModels;
using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private MainCategoryRepository mainCategoryRepository = new MainCategoryRepository();
        private SubCategoryRepository subCategoryRepository = new SubCategoryRepository();
        private FinalSubCategoryRepository finalSubCategoryRepository = new FinalSubCategoryRepository();
        
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getProducts(int id, int category)
        {
            if(category == 1)
            {
                mainCategoryRepository.Get(id);
                return View(mainCategoryRepository);
            }
            else if(category == 2)
            {
                subCategoryRepository.Get(id);
                return View(subCategoryRepository);
            }
            else
            {
                finalSubCategoryRepository.Get(id);
                return View(finalSubCategoryRepository);
            }
            
        }
        [HttpGet]
        public ActionResult manCategories(int id)
        {
            return View(mainCategoryRepository.Get(id));
        }

        [HttpGet]
        public ActionResult womenCategories(int id)
        {
            return View(mainCategoryRepository.Get(id));
        }

        [HttpGet]
        public ActionResult lifestylesCategories(int id)
        {
            return View(mainCategoryRepository.Get(id));
        }

        [HttpGet]
        public ActionResult saleProducts(int id)
        {
            return View(mainCategoryRepository.Get(id));
        }
    }
}