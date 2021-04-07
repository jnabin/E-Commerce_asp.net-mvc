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
    public class CartController : Controller
    {
        
        private CustomerRepository customermodel = new CustomerRepository();
        private ProductRepository product = new ProductRepository();
        private ProductSizeRepository productsizeModel = new ProductSizeRepository();
        private OrderRepository OrderModel = new OrderRepository();
        private ProductOrderRepository OrderProductModel = new ProductOrderRepository();
        // GET: Cart
        public ActionResult Index()
        {
            List<CartViewModel> model = new List<CartViewModel>();
            if (Session["cart"] != null)
            {
                model = (List<CartViewModel>)Session["cart"];
                for (int x = 0; x < model.Count; x++)
                {
                    if (product.Get(model[x].product.Product_id) == null)
                    {
                        model[x].outOfStock = "Out Of Stock";
                    }
                    else
                    {
                        model[x].outOfStock = "";
                    }
                }

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            List<CartViewModel> cartViewlist = new List<CartViewModel>();
            cartViewlist = (List<CartViewModel>)Session["cart"];
            cartViewlist.RemoveAt(id);
            Session["cart"] = cartViewlist;
            if(cartViewlist.Count == 0)
            {
                Session["cart"] = null;
                return Json("empty");
            }
            else
            {
                return Json("notempty");
            }
            


        }
        [HttpGet]
        public ActionResult Checkout()
        {
            if (Session["customer"]==null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                CheckOutViewModel checkOutViewModel = new CheckOutViewModel();
                checkOutViewModel.customer = customermodel.Get((int)Session["LoginID"]);
                checkOutViewModel.cartproductlist = (List<CartViewModel>)Session["cart"];
                return View(checkOutViewModel);
            }
            
        }
    }
}