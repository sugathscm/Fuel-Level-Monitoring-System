using FLMS;
using FLMS.BAL;
using FLMS.DAL;
using FLMS.UI;
using FLMS.UI.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WFM.UI.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly CityService cityService = new CityService();

        public CityController()
        {
        }

        public CityController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Citys
        public ActionResult Index(int? id)
        {            
            CityViewModel cityViewModel = new CityViewModel();
            if (id != null)
            {
                City city = cityService.GetCityById(id);

                cityViewModel = new CityViewModel()
                {
                    Id = city.Id,
                    IsActive = city.IsActive,
                    Name = city.Name,
                    DistrictName = city.District.Name,
                    DistrictId = city.DistrictId.Value
                };
            }

            ViewBag.DistrictList = new SelectList(genericService.GetList<District>(), "Id", "Name");

            return View(cityViewModel);
        }

        public ActionResult GetList()
        {
            List<City> list = cityService.GetCityList();

            List<CityViewModel> modelList = new List<CityViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new CityViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    DistrictName = item.District.Name
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(CityViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                City city = null;
                City oldCity = null;
                if (model.Id == 0)
                {
                    city = new City
                    {
                        Name = model.Name,
                        IsActive = true,
                        DistrictId = model.DistrictId
                    };

                    oldCity = new City();
                    oldData = new JavaScriptSerializer().Serialize(oldCity);
                    newData = new JavaScriptSerializer().Serialize(city);
                }
                else
                {
                    city = genericService.GetList<City>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldCity = genericService.GetList<City>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new City()
                    {
                        Id = oldCity.Id,
                        Name = oldCity.Name,
                        IsActive = oldCity.IsActive
                    });

                    city.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    city.IsActive = model.IsActive;
                    city.DistrictId = model.DistrictId;

                    newData = new JavaScriptSerializer().Serialize(new City()
                    {
                        Id = city.Id,
                        Name = city.Name,
                        IsActive = city.IsActive
                    });
                }

                genericService.SaveOrUpdate<City>(city, city.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Citys",
                //    NewData = newData,
                //    OldData = oldData,
                //    UpdatedOn = DateTime.Now,
                //    UserId = User.Identity.GetUserId()
                //});

                TempData["Message"] = ResourceData.SaveSuccessMessage;
            }
            catch (Exception ex)
            {
                TempData["Message"] = string.Format(ResourceData.SaveErrorMessage, ex.InnerException);
            }


            return RedirectToAction("Index", "City");
        }
    }
}