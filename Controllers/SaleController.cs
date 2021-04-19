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
    public class SaleController : Controller
    {
        private ProductRepository product = new ProductRepository();
        private ProductHistoryRepository producthis = new ProductHistoryRepository();
        private SaleRepository salerepo = new SaleRepository();
        private SaleHistoryRepository saleHistoryRepo = new SaleHistoryRepository();
        private ProductRepository productRepository = new ProductRepository();
        // GET: Sale
        public ActionResult Index()
        {
            return View(salerepo.getSaleProduct());
        }
        [HttpPost]
        public ActionResult AddProductToSale(Sale sale, int Product_id)
        {
            sale.StartDate = DateTime.Now;
            salerepo.Insert(sale);
            SaleHistory salehis = new SaleHistory();
            salehis.Amount = sale.Amount;
            salehis.StartDate = sale.StartDate;
            saleHistoryRepo.Insert(salehis);
            Product x = product.Get(Product_id);
            x.SaleID = sale.SaleID;
            product.Update(x); 
            ProductHistory xh = producthis.GetByProductNameCategory(product.Get(Product_id).Product_name, product.Get(Product_id).CategoryID);
            xh.SaleID = salehis.SaleID;
            producthis.Update(xh);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult StopSale(int id)
        {
            int saleid = Convert.ToInt32(product.Get(id).SaleID);
            Product obj = product.Get(id);
            obj.SaleID = null;
            product.Update(obj);

            salerepo.Delete(saleid); 

            return Json("success");
        }

        public ActionResult ProductSale()
        {
            return View(product.GetAll().Where(x => x.Sale != null));
        }
    }
}