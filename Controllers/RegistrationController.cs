using E_Commerce.Models;
using E_Commerce.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class RegistrationController : Controller
    {
        private CustomerRepository customerRepository = new CustomerRepository();
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Customer customer, HttpPostedFileBase file)
        {
            if (customerRepository.GetByName(customer.Username))
            {
                TempData["error"] = "Username already taken";
                return View();
            }
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
            customerRepository.Insert(customer);
            TempData["success"] = "Registration done!";
            return RedirectToAction("Index", "Login");
            
        }
    }

}