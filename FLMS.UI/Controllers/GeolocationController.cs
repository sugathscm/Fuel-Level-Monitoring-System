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
    public class GeolocationController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly GeolocationService geolocationService = new GeolocationService();

        public GeolocationController()
        {
        }

        public GeolocationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Geolocations
        public ActionResult Index(int? id)
        {
            GeolocationViewModel geolocationViewModel = new GeolocationViewModel();
            Geolocation geolocation = new Geolocation();
            if (id != null)
            {
                geolocation = geolocationService.GetGeolocationById(id);

                geolocationViewModel = new GeolocationViewModel()
                {
                    Id = geolocation.Id,
                    IsActive = geolocation.IsActive,
                    Address = geolocation.Address + " " + geolocation.City.Name,
                    CityName = geolocation.City.Name
                };
            }

            ViewBag.CityList = new SelectList(genericService.GetList<City>(), "Id", "Name");

            return View(geolocationViewModel);
        }

        public ActionResult GetList()
        {
            List<Geolocation> list = geolocationService.GetGeolocationList();

            List<GeolocationViewModel> modelList = new List<GeolocationViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new GeolocationViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Address = item.Address,
                    CityName = item.City.Name
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(GeolocationViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Geolocation geolocation = null;
                Geolocation oldGeolocation = null;
                if (model.Id == 0)
                {
                    geolocation = new Geolocation
                    {
                        Address = model.Address,
                        IsActive = true,
                        CityId = model.CityId
                    };

                    oldGeolocation = new Geolocation();
                    oldData = new JavaScriptSerializer().Serialize(oldGeolocation);
                    newData = new JavaScriptSerializer().Serialize(geolocation);
                }
                else
                {
                    geolocation = genericService.GetList<Geolocation>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldGeolocation = genericService.GetList<Geolocation>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Geolocation()
                    {
                        Id = oldGeolocation.Id,
                        Address = oldGeolocation.Address,
                        IsActive = oldGeolocation.IsActive
                    });

                    geolocation.Address = model.Address;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    geolocation.IsActive = model.IsActive;
                    geolocation.CityId = model.CityId;

                    newData = new JavaScriptSerializer().Serialize(new Geolocation()
                    {
                        Id = geolocation.Id,
                        Address = geolocation.Address,
                        IsActive = geolocation.IsActive
                    });
                }

                genericService.SaveOrUpdate<Geolocation>(geolocation, geolocation.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Geolocations",
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


            return RedirectToAction("Index", "Geolocation");
        }
    }
}