using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Extensions;

namespace UHub.Controllers
{
    public class DynamicScriptsController : Controller
    {

        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        protected override void HandleUnknownAction(string actionName)
        {
            var res = this.JavaScriptFromView();
            res.ExecuteResult(ControllerContext);
        }
    }
}