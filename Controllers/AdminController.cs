using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class AdminController : Controller
    {
        private ProductRepository productRepository = new ProductRepository();
        private SubCategoryRepository SubCategory = new SubCategoryRepository();
        private FinalSubCategoryRepository FinalSub = new FinalSubCategoryRepository();
        private MainCategoryRepository mainCategoryRepository = new MainCategoryRepository();
        private ActionResult CheckValidity()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            else
            {
                return null;
            }
        }
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddProduct()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
           
            if(Session["product"] != null)
            {
                if (Session["ProductError"] != null)
                {
                    TempData["ProductError"] = Session["ProductError"];
                    Session["ProductError"] = null;
                }
                TempData["product"] = Session["product"];
                Session["product"] = null;
                ViewData["MainCatagories"] = mainCategoryRepository.GetAll().ToList();
                return View((Product)TempData["product"]);
            }
            else
            {
                if (Session["ProductError"] != null)
                {
                    TempData["ProductError"] = Session["ProductError"];
                    Session["ProductError"] = null;
                }
                ViewData["MainCatagories"] = mainCategoryRepository.GetAll().ToList();
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase[] files)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            if (ModelState.IsValid)
            {
                if (product.SubCategoryID != null)
                {
                    product.CategoryID = SubCategory.Get((int)product.SubCategoryID).MCategory_id;
                }
                if (product.FinalSubCategoryID != null)
                {
                    product.SubCategoryID = FinalSub.Get((int)product.FinalSubCategoryID).SubCate_id;
                    product.CategoryID = SubCategory.Get((int)product.SubCategoryID).MCategory_id;
                }
                if (productRepository.GetByProductName(product.Product_name, (int)product.CategoryID))
                {
                    ViewData["errorm"] = "product name already exist!";
                    return Json(new
                    {
                        newUrl = Url.Action("AddProduct")
                    }
               );
                }
                String filename = "";
                //iterating through multiple file collection   
                foreach (HttpPostedFileBase file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            string path = System.IO.Path.Combine(Server.MapPath("~/content/image"), System.IO.Path.GetFileName(file.FileName));
                            file.SaveAs(path);
                            filename += System.IO.Path.GetFileName(file.FileName) + '|';
                        }
                        catch (Exception exp)
                        {
                            ViewBag.Message = "ERROR:" + exp.Message.ToString();
                            return Json(new
                            {
                                newUrl = Url.Action("AddProduct")
                            }
               );
                        }
                    }
                }
                product.AddedDate = DateTime.Now;
                product.ImageFile = filename.Remove(filename.Length - 1, 1);
                productRepository.Insert(product);
                if (product.SizeCategory == "other")
                {
                    return Json(new
                    {
                        newUrl = Url.Action("Index")
                    }
                );
                }
                else
                {
                    return Json(new
                    {
                        newUrl = Url.Action("AddSize", new { id = product.Product_id }) //Payment as Action; Process as Controller
                    }
                );

                }
            }
            ViewData["MainCatagories"] = mainCategoryRepository.GetAll().ToList();
            Session["ProductError"] = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            Session["product"] = product;
            return Json(new
            {
                newUrl = Url.Action("AddProduct") //Payment as Action; Process as Controller
            }
                );


        }
        public ActionResult AddSize(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            SizeViewModel model = new SizeViewModel();
            
            model.product = productRepository.Get(id);
            if(model.product.SizeCategory == "dress")
            {
                model.sizelist = new SelectList(
               new List<SelectListItem>
               {
                    new SelectListItem { Selected = true, Text = "Small", Value = "small"},
                    new SelectListItem { Selected = false, Text = "Medium", Value = "medium"},
                    new SelectListItem { Selected = false, Text = "Large", Value = "large"},
                    new SelectListItem { Selected = false, Text = "Extra Large", Value = "Extra Large"},
               }, "Value", "Text", 1);
            }else if(model.product.SizeCategory == "pant")
            {
                model.sizelist = new SelectList(
              new List<SelectListItem>
              {
                    new SelectListItem { Selected = true, Text = "28", Value = "28"},
                    new SelectListItem { Selected = false, Text = "30", Value = "30"},
                    new SelectListItem { Selected = false, Text = "32", Value = "32"},
                    new SelectListItem { Selected = false, Text = "34", Value = "34"},
                    new SelectListItem { Selected = false, Text = "34", Value = "36"},
                    new SelectListItem { Selected = false, Text = "34", Value = "38"},
              }, "Value", "Text", 1);
            }
            else
            {
                model.sizelist = new SelectList(
              new List<SelectListItem>
              {
                    new SelectListItem { Selected = true, Text = "38", Value = "38"},
                    new SelectListItem { Selected = false, Text = "39", Value = "39"},
                    new SelectListItem { Selected = false, Text = "40", Value = "40"},
                    new SelectListItem { Selected = false, Text = "41", Value = "41"},
                    new SelectListItem { Selected = false, Text = "42", Value = "42"},
                    new SelectListItem { Selected = false, Text = "43", Value = "43"},
              }, "Value", "Text", 1);
            }
           
            return View(model);
        }
        [HttpPost]
        public ActionResult AddSize(SizeViewModel sizeViewModel, int id)
        {
            ProductSizeRepository productSizeRP = new ProductSizeRepository();
            
            //sizeViewModel.productSizes.ProductID = id;
            //sizeViewModel.productSizes.Count = sizeViewModel.product.Onhand;
            foreach (var item in sizeViewModel.Sizes)
            {
                sizeViewModel.productSizes.SizeName = item;
                productSizeRP.Insert(sizeViewModel.productSizes);
            }
            return RedirectToAction("Index");
        }

    }
}