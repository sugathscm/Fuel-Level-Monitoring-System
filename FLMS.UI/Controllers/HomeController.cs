using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FLMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.TankCount = 3;
            ViewBag.TankLevels = "[{\"tankid\": \"K0001\", \"level\": 20, \"location\": \"kolonnawa\"}, {\"tankid\": \"K0002\", \"level\": 58, \"location\": \"kolonnawa\"}, {\"tankid\": \"O0001\", \"level\": 87, \"location\": \"Orugodawatta\"}]";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        public JsonResult GetTankLevels()
        {
            string output = "";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }
}