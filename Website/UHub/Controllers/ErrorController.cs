﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UHub.CoreLib.Extensions;

namespace UHub.Controllers
{
    public class ErrorController : Controller
    {
        private static Dictionary<int, string> CapturedCodes = new Dictionary<int, string>()
        {
            [400] = "The server cannot process the specified request",
            [401] = "User not authenticated",
            [403] = "User not authorized",
            [404] = "Requested resource not found",
            [500] = "An unexpected server error has occured",
            [501] = "Requested resource is not implemented",
            [502] = "Bad Gateway",
            [503] = "Requested service is not currently available",
            [510] = "Requested service has been removed"
        };


        // GET: Error
        public ActionResult Index()
        {
            //IMPORTANT
            //POST-CONDITION
            //
            //"ViewBag.Title" must be set
            //"ViewBag.ErrorMessage" must be set

            Response.TrySkipIisCustomErrors = true;

            var idObj = Url.RequestContext.RouteData.Values["id"];
            var idStr = idObj?.ToString() ?? "0";
            var valid = int.TryParse(idStr, out var id);

            //set default error code
            id = valid ? id : 400;



            //verify valid code
            if (!CapturedCodes.TryGetValue(id, out var message))
            {
                if (id >= 400 && id < 500)
                {
                    id = 400;
                }
                else if (id >= 500 && id < 600)
                {
                    id = 500;
                }
                else
                {
                    id = 400;
                }
                message = CapturedCodes[id];
            }



            //401 - 499
            if (id > 400 && id < 500)
            {
                ViewBag.Title = "Request Error " + id;
                ViewBag.ErrorMessage = message;
            }
            //500 - 599
            else if (id >= 500 && id < 600)
            {
                ViewBag.Title = "Server Error " + id;
                ViewBag.ErrorMessage = message;
            }
            else
            {
                ViewBag.Title = "Request Error " + id;
                ViewBag.ErrorMessage = CapturedCodes[400];
            }

            Response.StatusCode = id;
            return View();
        }
    }
}