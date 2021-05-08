using FLMS.BAL;
using FLMS.UI.Models;
using Microsoft.AspNet.Identity.Owin;
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
        private ApplicationUserManager _userManager;
        private readonly TankFuelLevelService tankFuelLevelService = new TankFuelLevelService();
        private readonly CityService cityService = new CityService();

        public HomeController()
        {
        }

        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        public ActionResult Index()
        {
            var tankLevels = tankFuelLevelService.GetFuelLevels(0);

            ViewBag.TankCount = 10;
            ViewBag.TankLevels = tankLevels;

            ViewBag.CityList = new SelectList(cityService.GetCityList(), "Id", "Name");
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


        public JsonResult GetTankLevels(string id)
        {
            var tankLevels = tankFuelLevelService.GetFuelLevels(int.Parse(id));
            ViewBag.TankCount = 1;
            return Json(tankLevels, JsonRequestBehavior.AllowGet);
        }
    }
}