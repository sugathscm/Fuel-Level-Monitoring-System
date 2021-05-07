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
    public class SensorDeviceController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly SensorDeviceService sensorDeviceService = new SensorDeviceService();

        public SensorDeviceController()
        {
        }

        public SensorDeviceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: SensorDevices
        public ActionResult Index(int? id)
        {
            SensorDeviceViewModel sensorDeviceViewModel = new SensorDeviceViewModel();
            if (id != null)
            {
                SensorDevice sensorDevice = sensorDeviceService.GetSensorDeviceById(id);

                sensorDeviceViewModel = new SensorDeviceViewModel()
                {
                    Id = sensorDevice.Id,
                    IsActive = sensorDevice.IsActive,
                    ModelNumber = sensorDevice.Model,
                    SerialNumber = sensorDevice.SerialNumber,
                    SupplierName = sensorDevice.Supplier.Name
                };
            }

            ViewBag.SupplierList = new SelectList(genericService.GetList<Supplier>(), "Id", "Name");

            return View(sensorDeviceViewModel);
        }

        public ActionResult GetList()
        {
            List<SensorDevice> list = sensorDeviceService.GetSensorDeviceList();

            List<SensorDeviceViewModel> modelList = new List<SensorDeviceViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new SensorDeviceViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    ModelNumber = item.Model,
                    SerialNumber = item.SerialNumber,
                    SupplierName = item.Supplier.Name
                });
            }
            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(SensorDeviceViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                SensorDevice sensorDevice = null;
                SensorDevice oldSensorDevice = null;
                if (model.Id == 0)
                {
                    sensorDevice = new SensorDevice
                    {
                        Model = model.ModelNumber,
                        SerialNumber = model.SerialNumber,
                        IsActive = true,
                        SupplierId = model.SupplierId
                    };

                    oldSensorDevice = new SensorDevice();
                    oldData = new JavaScriptSerializer().Serialize(oldSensorDevice);
                    newData = new JavaScriptSerializer().Serialize(sensorDevice);
                }
                else
                {
                    sensorDevice = genericService.GetList<SensorDevice>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldSensorDevice = genericService.GetList<SensorDevice>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new SensorDevice()
                    {
                        Id = oldSensorDevice.Id,
                        SerialNumber = oldSensorDevice.SerialNumber,
                        IsActive = oldSensorDevice.IsActive,
                        SupplierId = model.SupplierId
                    });

                    sensorDevice.SerialNumber = model.SerialNumber;
                    sensorDevice.Model = model.ModelNumber;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    sensorDevice.IsActive = model.IsActive;
                    sensorDevice.SupplierId = model.SupplierId;

                    newData = new JavaScriptSerializer().Serialize(new SensorDevice()
                    {
                        Id = sensorDevice.Id,
                        SerialNumber = sensorDevice.SerialNumber,
                        IsActive = sensorDevice.IsActive
                    });
                }

                genericService.SaveOrUpdate<SensorDevice>(sensorDevice, sensorDevice.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "SensorDevices",
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


            return RedirectToAction("Index", "SensorDevice");
        }
    }
}