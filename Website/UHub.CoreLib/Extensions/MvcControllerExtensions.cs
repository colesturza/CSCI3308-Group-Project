using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UHub.CoreLib.Extensions
{
    public static class MvcControllerExtensions
    {

        public static bool ViewExists(this Controller controller, string name)
        {
            ViewEngineResult result = ViewEngines.Engines.FindView(controller.ControllerContext, name, null);
            return (result.View != null);
        }

    }
}
