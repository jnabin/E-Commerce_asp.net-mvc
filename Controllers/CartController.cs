using E_Commerce.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            List<CartViewModel> model = new List<CartViewModel>();
            if (Session["cart"] != null)
            {
                model = (List<CartViewModel>)Session["cart"];
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
    }
}