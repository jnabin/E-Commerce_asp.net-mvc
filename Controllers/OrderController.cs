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
    public class OrderController : Controller
    {
        private ProductSizeRepository productsizeModel = new ProductSizeRepository();
        private ProfitRepository profitrepo = new ProfitRepository();
        private OrderRepository OrderModel = new OrderRepository();
        private ProductOrderRepository OrderProductModel = new ProductOrderRepository();
        private ProductHistoryRepository productHistory = new ProductHistoryRepository();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
            return View();
        }
        public ActionResult UpdateOrderInformation()
        {
            List<CartViewModel> cartlist = new List<CartViewModel>();
            cartlist = (List<CartViewModel>)Session["cart"];
            var date = DateTime.Now;
            foreach (var item in cartlist)
            {
                ProductSize psize = new ProductSize();
                psize = productsizeModel.Get(item.size.ProductSizeID);
                psize.Count = (psize.Count - item.count);
                if (psize.Count == 0)
                {
                    productsizeModel.Delete(psize.ProductSizeID);
                }
                else {
                    productsizeModel.Update(psize);
                }

                Order order = new Order();
                order.date = date;
                order.Quantity = item.count;
                order.totalAmount = (item.count * item.product.UnitPrice);
                order.Size = item.size.SizeName;
                order.PayMentMethod = (string)Session["paymentmethod"];
                OrderModel.Insert(order);
                OrderProduct orderproduct = new OrderProduct();
                orderproduct.CustomerID = (int)Session["LoginID"];
                orderproduct.OrderID = order.OrderID;
                orderproduct.ProductID = productHistory.GetByProductNameCategory(item.product.Product_name, item.product.CategoryID).Product_id;
                OrderProductModel.Insert(orderproduct);

                Profit profit = new Profit();
                profit.ProductOrderID = orderproduct.ProductOrderID;
                profit.ProfitAmount = (item.product.UnitPrice - item.product.Cost) * item.count;
                profitrepo.Insert(profit);
            }
            Session["cart"] = null;
           
            return RedirectToAction("SuccessView");
        }
        [HttpPost]
        public ActionResult SetPaymentMethod(String paymentmethod)
        {
            Session["paymentmethod"] = paymentmethod;
            return Json("Success");
        }
    }
}