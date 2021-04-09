using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.ReportContent;
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
        private ReviewRepository reviewrepo = new ReviewRepository();
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
            reviewrepo.DeleteReview(id);
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

        public ActionResult ReviewList()
        {
            return View(reviewrepo.GetAll());
        }

        public ActionResult GetManCategoryReportData()
        {
         int Jackets = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Jackets").Count();
         int Suits_Blazers = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Suits & Blazers").Count();
         int Hoodies = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Hoodies").Count();
            int SweatShirts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "SweatShirts").Count();
            int Sweater = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Sweater").Count();
            int Casual_Shirts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Casual Shirts").Count();
            int Polo = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Polo").Count();
            int T_Shirts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "T Shirts").Count();
            int Dress_Shirts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Dress Shirts").Count();
            int Jeans = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Jeans").Count();
            int Chinos = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Chinos").Count();
            int Dress_Pants = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Dress Pants").Count();
            int Shorts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Shorts").Count();
            int Joggers_Trousers = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Joggers/Trousers").Count();
            int Loafers = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Loafers").Count();
            int Sneakers = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Sneakers").Count();
            int Sandals = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Sandals").Count();
            int Boots = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Boots").Count();
            int Bags = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Bags").Count();
            int Caps_Hats = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Caps/Hats").Count();
            int Mask = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Mask").Count();
            int Umbrella_Mug = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Umbrella & Mug").Count();
            int Sunglass = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Sunglass").Count();
            int Wallet_Card = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Wallet/Card").Count();
          

            MenCategoryRatio obj = new MenCategoryRatio();
            obj.Bags = Bags;
            obj.Boots = Boots;
            obj.Caps_Hats = Caps_Hats;
            obj.Casual_Shirts = Casual_Shirts;
            obj.Chinos = Chinos;
            obj.Dress_Pants = Dress_Pants;
            obj.Dress_Shirts = Dress_Shirts;
            obj.Hoodies = Hoodies;
            obj.Jackets = Jackets;
            obj.Jeans = Jeans;
            obj.Joggers_Trousers = Joggers_Trousers;
            obj.Loafers = Loafers;
            obj.Mask = Mask;
            obj.Polo = Polo;
            obj.Sandals = Sandals;
            obj.Shorts = Shorts;
            obj.Sneakers = Sneakers;
            obj.Suits_Blazers = Suits_Blazers;
            obj.Sunglass = Sunglass;
            obj.Sweater = Sweater;
            obj.SweatShirts = SweatShirts;
            obj.Umbrella_Mug = Umbrella_Mug;
            obj.Wallet_Card = Wallet_Card;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWomenCategoryReportData()
        {
            int Overcoat = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Overcoat").Count();
            int Hoodies = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Hoodies" && x.MainCategory.Category_name == "Women").Count();
            int Jackets = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Jackets" && x.MainCategory.Category_name == "Women").Count();
            int Sweatshirt = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Sweatshirt" && x.MainCategory.Category_name == "Women").Count();
            int Ponchos = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Ponchos").Count();
            int Kammez_Kurtis = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Kammez/Kurtis").Count();
            int Fashion_Tops_Shirts = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Fashion Tops & Shirts").Count();
            int Shrugs_Duster = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Shrugs & Duster").Count();
            int Blazers = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Blazers" && x.MainCategory.Category_name == "Women").Count();
            int Jeans = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Jeans" && x.MainCategory.Category_name=="Women").Count();
            int Leggings = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Leggings").Count();
            int Zarzain_Pants = productRepository.GetAll().Where(x => x.FinalSubCategory.FinalSubCate_name == "Zarzain Pants").Count();
            int Scarves = productRepository.GetAll().Where(x => x.SubCatetory.SubCategory_name == "Scarves").Count();
            int Bags = productRepository.GetAll().Where(x => x.SubCatetory.SubCategory_name == "Bags" && x.MainCategory.Category_name == "Women").Count();
          
            WomenCategoryRatio obj = new WomenCategoryRatio();
            obj.Bags = Bags;
            obj.Blazers = Blazers;
            obj.Fashion_Tops_Shirts = Fashion_Tops_Shirts;
            obj.Hoodies = Hoodies;
            obj.Jackets = Jackets;
            obj.Jeans = Jeans;
            obj.Kammez_Kurtis = Kammez_Kurtis;
            obj.Leggings = Leggings;
            obj.Overcoat = Overcoat;
            obj.Ponchos = Ponchos;
            obj.Scarves = Scarves;
            obj.Shrugs_Duster = Shrugs_Duster;
            obj.Sweatshirt = Sweatshirt;
            obj.Zarzain_Pants = Zarzain_Pants;
            
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLifeStyleCategoryReportData()
        {
            int Luggage = productRepository.GetAll().Where(x => x.SubCatetory.SubCategory_name == "Luggage").Count();
            int Perfume = productRepository.GetAll().Where(x => x.SubCatetory.SubCategory_name == "Perfume").Count();
            int Sunglass = productRepository.GetAll().Where(x => x.SubCatetory.SubCategory_name == "Sunglass").Count();

            LifeStyleCategoryRatio obj = new LifeStyleCategoryRatio();
            obj.Luggage = Luggage;
            obj.Perfume = Perfume;
            obj.Sunglass = Sunglass;
            
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult mencategoryReportdetails()
        {
            return View();
        }
        public ActionResult womencategoryReportdetails()
        {
            return View();
        }
        public ActionResult lifestylecategoryReportdetails()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult ProductCount()
        {
            return PartialView(mainCategoryRepository.GetAll());
        }
    }


    
}

   