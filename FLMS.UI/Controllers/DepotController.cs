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
    public class DepotController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly DepotService depotService = new DepotService();
        private readonly GeolocationService geolocationService = new GeolocationService();

        public DepotController()
        {
        }

        public DepotController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Depots
        public ActionResult Index(int? id)
        {
            DepotViewModel depotViewModel = new DepotViewModel();
            if (id != null)
            {
                Depot depot = depotService.GetDepotById(id);

                depotViewModel = new DepotViewModel()
                {
                    Id = depot.Id,
                    IsActive = depot.IsActive,
                    Code = depot.Code,
                    GeolocationId = depot.GeolocationId,
                    GeolocationName = depot.Geolocation.Address,
                    DepotTypeId = depot.DepotTypeId,
                    DepotTypeName = depot.DepotType.Name
                };
            }

            ViewBag.DepotTypeList = new SelectList(genericService.GetList<DepotType>(), "Id", "Name");
            ViewBag.GeolocationList = new SelectList(geolocationService.GetGeolocationList().Select(g => new { Id = g.Id, Name = g.Address + ", " + g.City.Name}), "Id", "Name");

            return View(depotViewModel);
        }

        public ActionResult GetList()
        {
            List<Depot> list = depotService.GetDepotList();

            List<DepotViewModel> modelList = new List<DepotViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new DepotViewModel() { 
                    Id = item.Id, 
                    IsActive = item.IsActive, 
                    Code = item.Code, 
                    GeolocationName = item.Geolocation.Address, 
                    DepotTypeName = item.DepotType.Name 
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Depot model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Depot depot = null;
                Depot oldDepot = null;
                if (model.Id == 0)
                {
                    depot = new Depot
                    {
                        Code = model.Code,
                        GeolocationId = model.GeolocationId,
                        DepotTypeId = model.DepotTypeId,
                        IsActive = true
                    };

                    oldDepot = new Depot();
                    oldData = new JavaScriptSerializer().Serialize(oldDepot);
                    newData = new JavaScriptSerializer().Serialize(depot);
                }
                else
                {
                    depot = genericService.GetList<Depot>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDepot = genericService.GetList<Depot>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Depot()
                    {
                        Id = oldDepot.Id,
                        Code = oldDepot.Code,
                        IsActive = oldDepot.IsActive
                    });

                    depot.Code = model.Code;
                    depot.GeolocationId = model.GeolocationId;
                    depot.DepotTypeId = model.DepotTypeId;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    depot.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Depot()
                    {
                        Id = depot.Id,
                        Code = depot.Code,
                        IsActive = depot.IsActive
                    });
                }

                genericService.SaveOrUpdate<Depot>(depot, depot.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Depots",
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

            return RedirectToAction("Index", "Depot");
        }
    }
}