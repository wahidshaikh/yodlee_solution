using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yodleemodel.Collections;

namespace yodlee.pureprofile.com.Controllers
{
    public class YodleeController : Controller
    {
        //
        // GET: /Yodlee/

        public JsonResult Index()
        {
            YodleeAPI obj_yodlee = new YodleeAPI();
            string data = obj_yodlee.executeUserSearchRequest();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}
