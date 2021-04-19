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
    public class OrderController : CustomerBaseController
    {
        private ProductRepository productrepo = new ProductRepository();
        private ProductSizeRepository productsizeModel = new ProductSizeRepository();
        private ProfitRepository profitrepo = new ProfitRepository();
        private OrderRepository OrderModel = new OrderRepository();
        private ProductOrderRepository OrderProductModel = new ProductOrderRepository();
        private TempOrderRepository temporderrepo = new TempOrderRepository();
        private TempOrderProductRepository temporderproductrepo = new TempOrderProductRepository();
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
                if(item.product.SizeCategory == "other")
                {
                    Product p1 = new Product();
                    p1 = productrepo.Get(item.product.Product_id);
                    p1.OnHand -= item.count;
                    productrepo.Update(p1);
                }
                else
                {
                    ProductSize psize = new ProductSize();
                    psize = productsizeModel.Get(item.size.ProductSizeID);
                    psize.Count = (psize.Count - item.count);
                    if (psize.Count == 0)
                    {
                        productsizeModel.Delete(psize.ProductSizeID);
                    }
                    else
                    {
                        productsizeModel.Update(psize);
                    }
                }
             
                TempOrder order = new TempOrder();
                order.date = date;
                order.Quantity = item.count;
                if(item.product.Sale != null)
                {
                    var p = (double)item.product.UnitPrice;
                    var v = Convert.ToDouble(item.product.Sale.Amount) / 100;
                    var c1 = (p - (p * v));
                    order.totalAmount = (decimal)(item.count * c1);
                }
                else
                {
                    order.totalAmount = (item.count * item.product.UnitPrice);
                }
                if(item.product.SizeCategory != "other")
                {
                    order.Size = item.size.SizeName;
                }
               
                order.PayMentMethod = (string)Session["paymentmethod"];
                temporderrepo.Insert(order);

                TempOrderProduct orderproduct = new TempOrderProduct();
                orderproduct.CustomerID = (int)Session["LoginID"];
                orderproduct.OrderID = order.OrderID;
                orderproduct.ProductID = productHistory.GetByProductNameCategory(item.product.Product_name, item.product.CategoryID).Product_id;
                temporderproductrepo.Insert(orderproduct);

               /* Profit profit = new Profit();
                profit.ProductOrderID = orderproduct.ProductOrderID;
                if (item.product.Sale != null)
                {
                    var p = (double)item.product.UnitPrice;
                    var v = Convert.ToDouble(item.product.Sale.Amount) / 100;
                    var c1 = (p - (p * v));
                    profit.ProfitAmount = ((decimal)c1 - item.product.Cost) * item.count;
                }
                else
                {
                    profit.ProfitAmount = (item.product.UnitPrice - item.product.Cost) * item.count;
                }
                
               
                profitrepo.Insert(profit);*/
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