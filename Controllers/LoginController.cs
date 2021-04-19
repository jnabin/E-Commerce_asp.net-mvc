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
    public class LoginController : Controller
    {
        private CustomerRepository customerRepository = new CustomerRepository();
        private ManagerRepository managerrepo = new ManagerRepository();
        private AdminRepository adminRepository = new AdminRepository();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(LoginViewModel loginViewModel)
        {
            Boolean error = false;
            if (loginViewModel.Username == null)
            {
                error = true;
                TempData["userror"] = "username is required";
            }
            if (loginViewModel.Password == null)
            {
                error = true;
                TempData["passerror"] = "password is required";
            }
            if(error == true)
            {
                
                return RedirectToAction("Index");
            }
            
            Customer user = customerRepository.validateLogin(loginViewModel.Username, loginViewModel.Password);
            Admin auser = adminRepository.adminValidateLogin(loginViewModel.Username, loginViewModel.Password);
            Manager muser = managerrepo.managerValidateLogin(loginViewModel.Username, loginViewModel.Password);

          
            if(auser != null)
            {
                Session["admin"] = auser.Usertype;
                Session["adminLoginID"] = auser.AdminID;
                Session["adminUserName"] = auser.Username;
                return RedirectToAction("Index", "Admin");

                    
            }else if(muser != null)
            {
                Session["manager"] = muser.Usertype;
                Session["managerLoginID"] = muser.ManagerID;
                Session["managerUserName"] = muser.Username;
                return RedirectToAction("Index", "Manager");
            }else if(user != null)
            {
                Session["customer"] = user.usertype;
                Session["LoginID"] = user.CustomerID;
                Session["UserName"] = user.Username;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = "invalid username/password!";
                return RedirectToAction("Index");
            }
                
           
           
        }
    }
}