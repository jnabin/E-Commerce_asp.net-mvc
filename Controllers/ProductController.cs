using E_Commerce.Models;
using E_Commerce.Models.ViewModels;
using E_Commerce.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    
    public class ProductController : Controller
    {
        private ProductRepository product = new ProductRepository();
        private ProductOrderRepository OrderProduct = new ProductOrderRepository();
        private MainCategoryRepository MainCategoryrepo = new MainCategoryRepository();
        private ProductHistoryRepository productHistory = new ProductHistoryRepository();
        // GET: Product
        [HttpPost]
        
        public ActionResult GetDetails(int id)
        {  
            ProductViewModel productView = new ProductViewModel();
            productView.Product_id = product.Get(id).Product_id;
            if(product.Get(id).OnHand != null)
            {
                productView.Onhand = (int)product.Get(id).OnHand;
            }
           
            productView.ImageFile = product.Get(id).ImageFile;
            productView.Product_name = product.Get(id).Product_name;
            productView.Description = product.Get(id).Description;
            productView.CategoryID = product.Get(id).CategoryID;
            if (product.Get(id).Sale != null)
            {
                var p = (double)product.Get(id).UnitPrice;
                var v = Convert.ToDouble(product.Get(id).Sale.Amount) / 100;
                var c = (p - (p * v));
                productView.UnitPrice = (decimal)c;
            }
            else
            {
                productView.UnitPrice = product.Get(id).UnitPrice;
            }
                      
            productView.FinalSubCategoryID = product.Get(id).FinalSubCategoryID;
            productView.SubCategoryID = product.Get(id).SubCategoryID;
            productView.SizeCategory = product.Get(id).SizeCategory;
            List<string> sizename = new List<string>();
            var r = product.Get(id);
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
        public ActionResult addcart(int productid, int quantity, string sizename = "")
        {
            int v=0;
            if (sizename.Contains('|'))
            {
                v = Convert.ToInt32(sizename.Split('|')[0]);
            }
            else if(sizename != "")
            {
                v = Convert.ToInt32(sizename);
            }
            ProductSizeRepository sizeRepository = new ProductSizeRepository();
            if (v!=0 && sizeRepository.Get(v).Count < quantity)
            {
                return RedirectToAction(Session["actionname"].ToString(), new { id = Session["reid"], categoryname = Session["catname"] });
            }
            List<CartViewModel> cartviewlist = new List<CartViewModel>();
            if (Session["cart"] != null)
            {
                cartviewlist = (List<CartViewModel>)Session["cart"];
                for(int x=0; x<cartviewlist.Count; x++)
                {
                    if (product.Get(cartviewlist[x].product.Product_id) == null)
                    {
                        cartviewlist[x].outOfStock = "Out Of Stock";
                    }
                    else
                    {
                        cartviewlist[x].outOfStock = "";
                    }
                }
               
            }

            CartViewModel cartViewModel = new CartViewModel();
            
            Product model = product.Get(productid);
            var i = false;
            foreach (var item in cartviewlist)
            {
                if ((v == 0 && item.name == model.Product_name) || (v > 0 && item.name == model.Product_name && item.size.ProductID == model.Product_id))
                {
                    cartviewlist[cartviewlist.IndexOf(item)].count = quantity;
                    i = true;
                    break;
                }

            }
            if (i)
            {
                Session["cart"] = cartviewlist;
                return RedirectToAction("Index", "Cart");
            }
            else
            { 
                cartViewModel.product = model;
                cartViewModel.name = model.Product_name;
                cartViewModel.count = quantity;
                if (v > 0)
                {
                    cartViewModel.size = sizeRepository.Get(v);
                }
                cartviewlist.Add(cartViewModel);
                Session["cart"] = cartviewlist;

                return RedirectToAction("Index", "Cart");
            }
        }
        public ActionResult LifestyleProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                 return View(product.GetfromFinalCategory(id));
            }else if(categoryname == "m")
            {
                return View(product.GetfromMainCategory(id));
            }
            else
            {
                return View(product.GetfromSubCategory(id));
            }
            
        }

        public ActionResult Details(int id)
        {
            if (Session["LoginID"] != null)
            {
                List<OrderProduct> orderProductList = new List<OrderProduct>();
                ProductHistory producthis = productHistory.GetByProductNameCategory(product.Get(id).Product_name, product.Get(id).CategoryID);
                int pid = producthis.Product_id;
                orderProductList = OrderProduct.GetByCustomerId((int)Session["LoginID"], pid);
                if (orderProductList.Count==0)
                {
                    Session["notbuy"] = "In order to review you need to Buy this product";
                }
                else
                {
                    Session["notbuy"] = null;
                }

            }
            return View(product.Get(id));
        }

        public ActionResult ManProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                return View(product.GetfromFinalCategory(id));
            }
            else if (categoryname == "m")
            {
                return View(product.GetfromMainCategory(id));
            }
            else
            {
                return View(product.GetfromSubCategory(id));
            }
        }
        public ActionResult WomenProductlist(int id, string categoryname)
        {
            Session["reid"] = id;
            Session["catname"] = categoryname;
            if (categoryname == "f")
            {
                return View(product.GetfromFinalCategory(id));
            }
            else if (categoryname == "m")
            {
                return View(product.GetfromMainCategory(id));
            }
            else
            {
                return View(product.GetfromSubCategory(id));
            }
        }
        public ActionResult EditCartItem(int id)
        {
            List<CartViewModel> cartviewlist = new List<CartViewModel>();
            cartviewlist = (List<CartViewModel>)Session["cart"];
            Session["index"] = id;
            return View(cartviewlist[id]);
        }

        [HttpPost]
        public ActionResult updatecart(int cartindex, int quantity, string sizename = "")
        {
            int v = 0;
            if (sizename.Contains('|'))
            {
                v = Convert.ToInt32(sizename.Split('|')[0]);
            }
            else if(sizename != "")
            {
                v = Convert.ToInt32(sizename);
            }
            ProductSizeRepository sizeRepository = new ProductSizeRepository();
            if (v!=0 && sizeRepository.Get(v).Count < quantity)
            {
                return RedirectToAction(Session["actionname"].ToString(), new { id = Session["reid"], categoryname = Session["catname"] });
            }
            List<CartViewModel> cartviewlist = new List<CartViewModel>();
            if (Session["cart"] != null)
            {
                cartviewlist = (List<CartViewModel>)Session["cart"];
            }

            CartViewModel cartViewModel = new CartViewModel();
            cartviewlist[cartindex].count = quantity;
            cartviewlist[cartindex].size = sizeRepository.Get(v);
            Session["cart"] = cartviewlist;

                return RedirectToAction("Index", "Cart");
            
        }
        public ActionResult SortedProductList(string value)
         {
            string sortname = value.Split('|')[0];
            int id = Convert.ToInt32 (value.Split('|')[1]);
            string categoryname = value.Split('|')[2];
            Session["reid"] = id;
            Session["catname"] = categoryname;
            List<SortViewModel> sortmodel = new List<SortViewModel>();
            if (sortname == "atoz")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = product.GetfromFinalCategory(id).OrderBy(x => x.Product_name);
                    foreach(var item in sortedproduct)
                    {
                        sortmodel.Add( new SortViewModel() {
                            Product_name = item.Product_name,
                            Product_id  =item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        }  );
                    }
                }
                else if (categoryname == "m")
                {
                    var sortedproduct = product.GetfromMainCategory(id).OrderBy(x => x.Product_name);
                    foreach(var item in sortedproduct)
                    {
                        sortmodel.Add( new SortViewModel() {
                            Product_name = item.Product_name,
                            Product_id  =item.Product_id,
                            UnitPrice = item.UnitPrice,
                            ImageFile = item.ImageFile
                        }  );
                    }
                }
                else
                {
                    var sortedproduct = product.GetfromSubCategory(id).OrderBy(x => x.Product_name);
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
            else if(sortname == "ztoa"){
                if (categoryname == "f")
                {
                    var sortedproduct = product.GetfromFinalCategory(id).OrderByDescending(x => x.Product_name);
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
                    var sortedproduct = product.GetfromMainCategory(id).OrderByDescending(x => x.Product_name);
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
                    var sortedproduct = product.GetfromSubCategory(id).OrderByDescending(x => x.Product_name);
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
            else if(sortname == "lowtohigh")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = product.GetfromFinalCategory(id).OrderBy(x => x.UnitPrice);
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
                    var sortedproduct = product.GetfromMainCategory(id).OrderBy(x => x.UnitPrice);
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
                    var sortedproduct = product.GetfromSubCategory(id).OrderBy(x => x.UnitPrice);
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
            else if(sortname == "hightolow")
            {
                if (categoryname == "f")
                {
                    var sortedproduct = product.GetfromFinalCategory(id).OrderByDescending(x => x.UnitPrice);
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
                    var sortedproduct = product.GetfromMainCategory(id).OrderByDescending(x => x.UnitPrice);
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
                    var sortedproduct = product.GetfromSubCategory(id).OrderByDescending(x => x.UnitPrice);
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
                    var sortedproduct = product.GetfromFinalCategory(id);
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
                    var sortedproduct = product.GetfromMainCategory(id);
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
                    var sortedproduct = product.GetfromSubCategory(id);
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

        [HttpGet]
        public ActionResult saleProducts(int id)
        {
            return View(MainCategoryrepo.Get(id));
        }
    }
}