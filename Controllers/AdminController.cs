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
        private ProductHistoryRepository producthistory = new ProductHistoryRepository();
        private ProductSizeHistoryRepository sizehistory = new ProductSizeHistoryRepository();
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
            return View(productRepository.ProductsWithoutSize());
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
        public ActionResult ValidateProductname(int category, string name )
        {
            Boolean product = productRepository.GetByProductName(name, category);
            if (product)
            {
                return Json("yes");
            }
            else
            {
                return Json("No");
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

                ProductHistory Phistory = new ProductHistory();
                Phistory.AddedDate = product.AddedDate;
                Phistory.Product_name = product.Product_name;
                Phistory.UnitPrice = product.UnitPrice;
                Phistory.Description = product.Description;
                Phistory.ImageFile = product.ImageFile;
                Phistory.Cost = product.Cost;
                Phistory.CategoryID = product.CategoryID;
                Phistory.SubCategoryID = product.SubCategoryID;
                Phistory.FinalSubCategoryID = product.FinalSubCategoryID;
                Phistory.SizeCategory = product.SizeCategory;
                producthistory.Insert(Phistory);
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
        [HttpGet]
        public ActionResult AddSize(int id)
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("index", "Home");
            }
            SizeViewModel model = new SizeViewModel();
            List<string> sizes = new List<string>();
            model.product = productRepository.Get(id);
            if(model.product.SizeCategory == "dress")
            {
                sizes.Add("Small");
                sizes.Add("Medium");
                sizes.Add("Large");
                sizes.Add("Extra-Large");
                model.sizelist = sizes;
               
            }else if(model.product.SizeCategory == "pant")
            {
                sizes.Add("28");
                sizes.Add("30");
                sizes.Add("32");
                sizes.Add("34");
                sizes.Add("36");
                sizes.Add("38");
                model.sizelist = sizes;
            }
            else
            {
                sizes.Add("38");
                sizes.Add("39");
                sizes.Add("40");
                sizes.Add("41");
                sizes.Add("42");
                sizes.Add("43");
                model.sizelist = sizes;
            }
           
            return View(model);
        }
        [HttpPost]
        public ActionResult AddSize(SizeViewModel sizeViewModel, int id, List<int> Count)
        {
            ProductSizeRepository productSizeRP = new ProductSizeRepository();
            var model = sizeViewModel.productSizes;
           
            //sizeViewModel.productSizes.ProductID = id;
            //sizeViewModel.productSizes.Count = sizeViewModel.product.Onhand;
            for (int i=0;i< Count.Count; i++) {
                sizeViewModel.productSizes = model;
                sizeViewModel.productSizes.SizeName = sizeViewModel.Sizes.ElementAt(i);
                sizeViewModel.productSizes.Count = Count.ElementAt(i);
                productSizeRP.Insert(sizeViewModel.productSizes);

                ProductSizeHistory sizemodelhistory = new ProductSizeHistory();
                sizemodelhistory.Count = sizeViewModel.productSizes.Count;
                sizemodelhistory.ProductID =  sizehistory.GetByProductNameCategory(productRepository.Get(id).Product_name, productRepository.Get(id).CategoryID).Product_id;
                sizemodelhistory.SizeName = sizeViewModel.productSizes.SizeName;
                sizemodelhistory.Count = sizeViewModel.productSizes.Count;
                sizehistory.Insert(sizemodelhistory);
            }
            return RedirectToAction("Index");
        }

        public ActionResult DeleteProductFromSizesNotAvailable(int id)
        {
            productRepository.Delete(id);
            if(productRepository.ProductsWithoutSize().Count == 0)
            {
                return Json("empty");
            }
            else
            {
                return Json("not empty");
            }
            
        }
        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            return View();
        }

        public ActionResult getProducts(int id, int category)
        {
            if (category == 1)
            {
                mainCategoryRepository.Get(id);
                return View(mainCategoryRepository);
            }
            else if (category == 2)
            {
                SubCategory.Get(id);
                return View(SubCategory);
            }
            else
            {
                FinalSub.Get(id);
                return View(FinalSub);
            }

        }
        public ActionResult ProductDetails(int id)
        {

            return View(productRepository.Get(id));
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


        public ActionResult LifestyleProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                return View(productRepository.GetfromFinalCategory(id));
            }
            else if (categoryname == "m")
            {
                return View(productRepository.GetfromMainCategory(id));
            }
            else
            {
                return View(productRepository.GetfromSubCategory(id));
            }

        }

        public ActionResult ManProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                return View(productRepository.GetfromFinalCategory(id));
            }
            else if (categoryname == "m")
            {
                return View(productRepository.GetfromMainCategory(id));
            }
            else
            {
                return View(productRepository.GetfromSubCategory(id));
            }
        }
        public ActionResult WomenProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                return View(productRepository.GetfromFinalCategory(id));
            }
            else if (categoryname == "m")
            {
                return View(productRepository.GetfromMainCategory(id));
            }
            else
            {
                return View(productRepository.GetfromSubCategory(id));
            }
        }

        [HttpPost]
        public ActionResult GetDetails(int id)
        {
            ProductViewModel productView = new ProductViewModel();
            productView.Product_id = productRepository.Get(id).Product_id;
            productView.ImageFile = productRepository.Get(id).ImageFile;
            productView.Product_name = productRepository.Get(id).Product_name;
            productView.Description = productRepository.Get(id).Description;
            productView.CategoryID = productRepository.Get(id).CategoryID;

            productView.UnitPrice = productRepository.Get(id).UnitPrice;
            productView.FinalSubCategoryID = productRepository.Get(id).FinalSubCategoryID;
            productView.SubCategoryID = productRepository.Get(id).SubCategoryID;
            productView.SizeCategory = productRepository.Get(id).SizeCategory;
            List<string> sizename = new List<string>();
            var r = productRepository.Get(id);
            var f = r.ProductSizes;
            foreach (var item in f)
            {
                if (item.Count == 0)
                {
                    continue;
                }
                sizename.Add(item.SizeName);
            }
            List<int> sizeid = new List<int>();
            foreach (var item in f)
            {
                if (item.Count > 0)
                {
                    sizeid.Add(item.ProductSizeID);
                }

            }

            List<int> sizecount = new List<int>();
            foreach (var item in f)
            {
                if (item.Count > 0)
                {
                    sizecount.Add(item.Count);
                }

            }
            productView.sizecount = sizecount;
            productView.sizename = sizename;
            productView.sizeid = sizeid;
            return Json(productView);
        }

        [HttpPost]
        public ActionResult UpdateProduct(FormCollection formCollection, HttpPostedFileBase[] files)
        {
            String filename = "";
            Product product = new Product();
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
                            newUrl = Url.Action("Index")
                        }
           );
                    }
                }
                else
                {
                    filename = productRepository.Get(Convert.ToInt32(formCollection["Product_id"])).ImageFile;
                    break;
                }
                }
             
            product.ImageFile = filename;
            foreach(var key in formCollection.AllKeys)
            {
                if(key == "Product_id")
                {
                    product.Product_id = Convert.ToInt32(formCollection[key]);
                }
                else if(key == "Product_name")
                {
                    product.Product_name = formCollection[key];
                }
                else if (key == "Description")
                {
                    product.Description = formCollection[key];
                }
                else if (key == "UnitPrice")
                {
                    product.UnitPrice = (Convert.ToDecimal(formCollection[key]));
                }
            }
            ProductHistory Phistory = new ProductHistory();
            Phistory.Product_id = sizehistory.GetByProductNameCategory(productRepository.Get(product.Product_id).Product_name, productRepository.Get(product.Product_id).CategoryID).Product_id;
            Phistory.Product_name = product.Product_name;
            Phistory.UnitPrice = product.UnitPrice;
            Phistory.Description = product.Description;
            Phistory.Cost = product.Cost;
            Phistory.CategoryID = product.CategoryID;
            Phistory.ImageFile = product.ImageFile;
            Phistory.SubCategoryID = product.SubCategoryID;
            Phistory.FinalSubCategoryID = product.FinalSubCategoryID;
            Phistory.SizeCategory = product.SizeCategory;
            producthistory.PartialHistoryUpdate(Phistory);
            productRepository.PartialUpdate(product);
            return View("Index");
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            productRepository.DeleteSize(id);
            productRepository.Delete(id);
            return Json("done");
        }
        [HttpPost]
        public ActionResult SortedProductList(string value)
        {
            string sortname = value.Split('|')[0];
            int id = Convert.ToInt32(value.Split('|')[1]);
            string categoryname = value.Split('|')[2];
            Session["reid"] = id;
            Session["catname"] = categoryname;
            List<SortViewModel> sortmodel = new List<SortViewModel>();
            if (sortname == "atoz")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = productRepository.GetfromFinalCategory(id).OrderBy(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else if (categoryname == "m")
                {
                    var sortedproduct = productRepository.GetfromMainCategory(id).OrderBy(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else
                {
                    var sortedproduct = productRepository.GetfromSubCategory(id).OrderBy(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
            }
            else if (sortname == "ztoa")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = productRepository.GetfromFinalCategory(id).OrderByDescending(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }

                }
                else if (categoryname == "m")
                {
                    var sortedproduct = productRepository.GetfromMainCategory(id).OrderByDescending(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else
                {
                    var sortedproduct = productRepository.GetfromSubCategory(id).OrderByDescending(x => x.Product_name);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
            }
            else if (sortname == "lowtohigh")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = productRepository.GetfromFinalCategory(id).OrderBy(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }

                }
                else if (categoryname == "m")
                {
                    var sortedproduct = productRepository.GetfromMainCategory(id).OrderBy(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else
                {
                    var sortedproduct = productRepository.GetfromSubCategory(id).OrderBy(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
            }
            else if (sortname == "hightolow")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = productRepository.GetfromFinalCategory(id).OrderByDescending(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }

                }
                else if (categoryname == "m")
                {
                    var sortedproduct = productRepository.GetfromMainCategory(id).OrderByDescending(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else
                {
                    var sortedproduct = productRepository.GetfromSubCategory(id).OrderByDescending(x => x.UnitPrice);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
            }
            else if (sortname == "default")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = productRepository.GetfromFinalCategory(id);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }

                }
                else if (categoryname == "m")
                {
                    var sortedproduct = productRepository.GetfromMainCategory(id);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
                else
                {
                    var sortedproduct = productRepository.GetfromSubCategory(id);
                    foreach (var item in sortedproduct)
                    {
                        sortmodel.Add(new SortViewModel()
                        {
                            Product_name = item.Product_name,
                            Product_id = item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        });
                    }
                }
            }

            return Json(sortmodel);
        }
    }


    
}

   