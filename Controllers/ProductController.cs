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
        // GET: Product
        [HttpPost]
        
        public ActionResult GetDetails(int id)
        {  
            ProductViewModel productView = new ProductViewModel();
            productView.Product_id = product.Get(id).Product_id;
            productView.ImageFile = product.Get(id).ImageFile;
            productView.Product_name = product.Get(id).Product_name;
            productView.Description = product.Get(id).Description;
            productView.CategoryID = product.Get(id).CategoryID;
            productView.Onhand = product.Get(id).Onhand;
            productView.UnitPrice = product.Get(id).UnitPrice;            
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
            else
            {
                v = Convert.ToInt32(sizename);
            }
            ProductSizeRepository sizeRepository = new ProductSizeRepository();
            if (sizeRepository.Get(v).Count < quantity)
            {
                return RedirectToAction(Session["actionname"].ToString(), new { id = Session["reid"], categoryname = Session["catname"] });
            }
            List<CartViewModel> cartviewlist = new List<CartViewModel>();
            if (Session["cart"] != null)
            {
                cartviewlist = (List<CartViewModel>)Session["cart"];
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
        public ActionResult WomanProductlist(int id, string categoryname)
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
            else
            {
                v = Convert.ToInt32(sizename);
            }
            ProductSizeRepository sizeRepository = new ProductSizeRepository();
            if (sizeRepository.Get(v).Count < quantity)
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
    }
}