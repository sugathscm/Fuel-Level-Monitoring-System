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
    public class VehicleController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly VehicleService vehicleService = new VehicleService();
        private readonly GPSDeviceService gPSDeviceService = new GPSDeviceService();

        public VehicleController()
        {
        }

        public VehicleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Vehicles
        public ActionResult Index(int? id)
        {
            VehicleViewModel vehicleViewModel = new VehicleViewModel();
            if (id != null)
            {
                Vehicle vehicle = vehicleService.GetVehicleById(id);

                vehicleViewModel = new VehicleViewModel()
                {
                    Id = vehicle.Id,
                    IsActive = vehicle.IsActive,
                    VehicleNo = vehicle.VehicleNo,
                    Capacity = vehicle.Capacity,
                    GPSDeviceId = vehicle.GPSDeviceId,
                    GPSDeviceSN = vehicle.GPSDevice.SerialNumber
                };
            }

            ViewBag.GPSDeviceList = new SelectList(gPSDeviceService.GetGPSDeviceList().Select(g => new { Id = g.Id, Name = g.SerialNumber }), "Id", "Name");

            return View(vehicleViewModel);
        }

        public ActionResult GetList()
        {
            List<Vehicle> list = vehicleService.GetVehicleList();

            List<VehicleViewModel> modelList = new List<VehicleViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new VehicleViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    VehicleNo = item.VehicleNo,
                    Capacity = item.Capacity,
                    GPSDeviceId = item.GPSDeviceId,
                    GPSDeviceSN = item.GPSDevice.SerialNumber
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(VehicleViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Vehicle vehicle = null;
                Vehicle oldVehicle = null;
                if (model.Id == 0)
                {
                    vehicle = new Vehicle
                    {
                        VehicleNo = model.VehicleNo,
                        Capacity = model.Capacity,
                        IsActive = true,
                        GPSDeviceId = model.GPSDeviceId
                    };

                    oldVehicle = new Vehicle();
                    oldData = new JavaScriptSerializer().Serialize(oldVehicle);
                    newData = new JavaScriptSerializer().Serialize(vehicle);
                }
                else
                {
                    vehicle = genericService.GetList<Vehicle>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldVehicle = genericService.GetList<Vehicle>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Vehicle()
                    {
                        Id = oldVehicle.Id,
                        Capacity = oldVehicle.Capacity,
                        IsActive = oldVehicle.IsActive,
                        GPSDeviceId = model.GPSDeviceId
                    });

                    vehicle.VehicleNo = model.VehicleNo;
                    vehicle.Capacity = model.Capacity;
                    vehicle.GPSDeviceId = model.GPSDeviceId;
                    vehicle.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Vehicle()
                    {
                        Id = vehicle.Id,
                        VehicleNo = vehicle.VehicleNo,
                        IsActive = vehicle.IsActive
                    });
                }

                genericService.SaveOrUpdate<Vehicle>(vehicle, vehicle.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Vehicles",
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

            return RedirectToAction("Index", "Vehicle");
        }
    }
}