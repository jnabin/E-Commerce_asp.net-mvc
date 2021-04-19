using E_Commerce.Models;
using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class ManagerController : ManagerBaseController
    {
        private TempOrderProductRepository temporderProductRepository = new TempOrderProductRepository();
        private TempOrderRepository temporderrepo = new TempOrderRepository();
        private OrderRepository orderrepo = new OrderRepository();
        private ProductOrderRepository productorderrepo = new ProductOrderRepository();
        private ProfitRepository profitrepo = new ProfitRepository();
        private CustomerRepository customerrepo = new CustomerRepository();

        private ProductRepository productrepo = new ProductRepository();
        private ProductHistoryRepository producthis = new ProductHistoryRepository();
        // GET: Manager
        public ActionResult Index()
        {
            return View(temporderProductRepository.GetAll());
        }
        [HttpPost]
        public ActionResult Confirmorder(int id)
        {
            TempOrderProduct temporderProduct = temporderProductRepository.Get(id);
            TempOrder torder = temporderrepo.Get(temporderProductRepository.Get(id).OrderID);
            temporderProductRepository.Delete(temporderProductRepository.Get(id).ProductOrderID);
            temporderrepo.Delete(temporderProduct.OrderID);


            Order order = new Order();
            order.date = DateTime.Now;
            order.PayMentMethod = torder.PayMentMethod;
            order.Quantity = torder.Quantity;
            order.Size = torder.Size;
            order.totalAmount = torder.totalAmount;
            orderrepo.Insert(order);


            OrderProduct orderProduct = new OrderProduct();
            orderProduct.ProductID = temporderProduct.ProductID;
            orderProduct.CustomerID = temporderProduct.CustomerID;
            orderProduct.OrderID = order.OrderID;
            productorderrepo.Insert(orderProduct);

            Profit profit = new Profit();
           
            ProductHistory phis = producthis.Get(orderProduct.ProductID);
            profit.ProductOrderID = orderProduct.ProductOrderID;
            if (phis.Sale != null)
            {
                var p = (double)phis.UnitPrice;
                var v = Convert.ToDouble(phis.Sale.Amount) / 100;
                var c1 = (p - (p * v));
                profit.ProfitAmount = ((decimal)c1 - phis.Cost) * order.Quantity;
            }
            else
            {
                profit.ProfitAmount = (phis.UnitPrice - phis.Cost) * order.Quantity;
            }
                
               
            profitrepo.Insert(profit);

            return Json("success");
        }

        [HttpPost]
        public ActionResult Deleteorder(int id)
        {
            TempOrderProduct temporderProduct = temporderProductRepository.Get(id);
            TempOrder torder = temporderrepo.Get(temporderProductRepository.Get(id).OrderID);
            temporderProductRepository.Delete(temporderProductRepository.Get(id).ProductOrderID);
            temporderrepo.Delete(torder.OrderID);

            return Json("success");
        }

        public ActionResult DetailsFromHistory(int id)
        {
            return View(producthis.Get(id));

        }

        public ActionResult CustomerDetails(int id)
        {
            return View(customerrepo.Get(id));
        }
    }
}