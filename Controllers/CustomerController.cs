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
    public class CustomerController : Controller
    {
        private CustomerRepository customerRepository = new CustomerRepository();
        private ReviewRepository reviewModel = new ReviewRepository();
        private ProductOrderRepository productorder = new ProductOrderRepository();
        // GET: Customer
        public ActionResult Index()
        {
            if(Session["LoginID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View(customerRepository.Get((int)Session["LoginID"]));
        }

        [HttpPost]
        public ActionResult updateInfo(Customer customer)
        {
           
                if (Session["existUser"].ToString() != customer.Username && customerRepository.GetByName(customer.Username))
                {
                    TempData["error"] = "Username already taken";
                    return RedirectToAction("Index");
                }
                customer.usertype = "customer";
                customerRepository.Update(customer);
                return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult updateInfoDuringCheckout(Customer customer)
        {
            customerRepository.Update(customer);
            return Json("Success");
        }


        [HttpPost]
        public ActionResult updateImage(Customer customer, HttpPostedFileBase file)
        {
            
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = System.IO.Path.Combine(Server.MapPath("~/content/image"), System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    customer.ImageFile = System.IO.Path.GetFileName(file.FileName);
                }
                catch (Exception exp)
                {
                    ViewBag.Message = "ERROR:" + exp.Message.ToString();
                    return View();
                }
            }
            customer.usertype = "customer";
            customerRepository.Update(customer);
            
            return RedirectToAction("Index");

        }
        public ActionResult AddReview(Review review)
        {
            reviewModel.Insert(review);
            return Json("success");
        }
        public ActionResult GetOrderReportData()
        {
            int MenCategory = (productorder.GetAll()).Where(x => x.CustomerID == (int)Session["LoginId"] && x.ProductHistory.MainCategory.Category_name == "Men").Count();
            int WomenCategory = (productorder.GetAll()).Where(x => x.CustomerID == (int)Session["LoginId"] && x.ProductHistory.MainCategory.Category_name == "Women").Count();
            int LifeStyleCategory = (productorder.GetAll()).Where(x => x.CustomerID == (int)Session["LoginId"] && x.ProductHistory.MainCategory.Category_name == "Life Style").Count();
            int SaleCategory = (productorder.GetAll()).Where(x => x.CustomerID == (int)Session["LoginId"] && x.ProductHistory.MainCategory.Category_name == "Sale").Count();

            Ratio obj = new Ratio();
            obj.MenCategory = MenCategory;
            obj.WomenCategory = WomenCategory;
            obj.LifeStyleCategory = LifeStyleCategory;
            obj.SaleCategory = SaleCategory;

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [ChildActionOnly]
        public ActionResult GetOrders()
        {
            return PartialView(productorder.getordersbycid((int)Session["LoginId"]));
        }
        [HttpPost]
        public ActionResult UpdatePassword(ChangePasswordViewModel customer)
        {
            Customer realcustomer = customerRepository.Get(customer.CustomerID);
            if(realcustomer.password == customer.password)
            {
                realcustomer.password = customer.newpassword;
                customerRepository.Update(realcustomer);
            }
            return Json("success");
        }
    }
}