using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Commerce.Controllers
{
    public class AdminBaseController : Controller
    {
        // GET: AdminBase
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if(filterContext.HttpContext.Session["admin"] == null)
            {
                 Response.Redirect("/Home/Index");
            }
        }
    }
}