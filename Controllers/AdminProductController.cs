using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class AdminProductController : AdminBaseController
    {
        private ProductHistoryRepository producthistory = new ProductHistoryRepository();
        private ProductRepository productrepo = new ProductRepository();
        private ReviewRepository reviewrepo = new ReviewRepository();
        public ActionResult DetailsFromHistory(int id)
        {
            return View(producthistory.Get(id));

        }

        public ActionResult ReviewListbyProduct(int id)
        {
            return View(reviewrepo.GetAll().Where(x => x.ProductID == id));
        }
        [ChildActionOnly]
        public ActionResult GetTotalOrder(int id)
        {
            return PartialView(producthistory.GetByProductNameCategory(productrepo.Get(id).Product_name, productrepo.Get(id).CategoryID) );
        }

        public ActionResult OrderListbyProduct(int id)
        {
            return View(producthistory.GetByProductNameCategory(productrepo.Get(id).Product_name, productrepo.Get(id).CategoryID).OrderProducts);
        }
    }
}