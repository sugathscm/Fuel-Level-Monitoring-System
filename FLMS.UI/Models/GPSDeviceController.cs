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
    public class GPSDeviceController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly GPSDeviceService gpsDeviceService = new GPSDeviceService();

        public GPSDeviceController()
        {
        }

        public GPSDeviceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: GPSDevices
        public ActionResult Index(int? id)
        {
            GPSDeviceViewModel gpsDeviceViewModel = new GPSDeviceViewModel();
            if (id != null)
            {
                GPSDevice gpsDevice = gpsDeviceService.GetGPSDeviceById(id);

                gpsDeviceViewModel = new GPSDeviceViewModel()
                {
                    Id = gpsDevice.Id,
                    IsActive = gpsDevice.IsActive,
                    ModelNumber = gpsDevice.Model,
                    SerialNumber = gpsDevice.SerialNumber,
                    SupplierName = gpsDevice.Supplier.Name
                };
            }

            ViewBag.SupplierList = new SelectList(genericService.GetList<Supplier>(), "Id", "Name");

            return View(gpsDeviceViewModel);
        }

        public ActionResult GetList()
        {
            List<GPSDevice> list = gpsDeviceService.GetGPSDeviceList();

            List<GPSDeviceViewModel> modelList = new List<GPSDeviceViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new GPSDeviceViewModel()
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
        public ActionResult SaveOrUpdate(GPSDeviceViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                GPSDevice gpsDevice = null;
                GPSDevice oldGPSDevice = null;
                if (model.Id == 0)
                {
                    gpsDevice = new GPSDevice
                    {
                        Model = model.ModelNumber,
                        SerialNumber = model.SerialNumber,
                        IsActive = true,
                        SupplierId = model.SupplierId
                    };

                    oldGPSDevice = new GPSDevice();
                    oldData = new JavaScriptSerializer().Serialize(oldGPSDevice);
                    newData = new JavaScriptSerializer().Serialize(gpsDevice);
                }
                else
                {
                    gpsDevice = genericService.GetList<GPSDevice>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldGPSDevice = genericService.GetList<GPSDevice>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new GPSDevice()
                    {
                        Id = oldGPSDevice.Id,
                        SerialNumber = oldGPSDevice.SerialNumber,
                        IsActive = oldGPSDevice.IsActive,
                        SupplierId = model.SupplierId
                    });

                    gpsDevice.SerialNumber = model.SerialNumber;
                    gpsDevice.Model = model.ModelNumber;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    gpsDevice.IsActive = model.IsActive;
                    gpsDevice.SupplierId = model.SupplierId;

                    newData = new JavaScriptSerializer().Serialize(new GPSDevice()
                    {
                        Id = gpsDevice.Id,
                        SerialNumber = gpsDevice.SerialNumber,
                        IsActive = gpsDevice.IsActive
                    });
                }

                genericService.SaveOrUpdate<GPSDevice>(gpsDevice, gpsDevice.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "GPSDevices",
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


            return RedirectToAction("Index", "GPSDevice");
        }
    }
}