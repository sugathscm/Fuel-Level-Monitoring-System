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
    public class DepotTankController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly DepotTankService depotTankService = new DepotTankService();
        private readonly DepotService depotService = new DepotService();
        private readonly TankService tankService = new TankService();
        private readonly SensorDeviceService sensorDeviceService = new SensorDeviceService();

        public DepotTankController()
        {
        }

        public DepotTankController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: DepotTanks
        public ActionResult Index(int? id)
        {
            DepotTankViewModel depotViewModel = new DepotTankViewModel();
            if (id != null)
            {
                DepotTank depot = depotTankService.GetDepotTankById(id);

                depotViewModel = new DepotTankViewModel()
                {
                    Id = depot.Id,
                    DepotId = depot.DepotId,
                    DepotCode = depot.Depot.Code,
                    TankId = depot.TankId,
                    TankCode = depot.Tank.Code,
                    SensorDeviceId = depot.SensorDeviceId,
                    SensorDeviceSN = depot.SensorDevice.SerialNumber
                };
            }

            ViewBag.DepotList = new SelectList(depotService.GetDepotList().Select(g => new { Id = g.Id, Name = g.Code }), "Id", "Name");
            ViewBag.TankList = new SelectList(tankService.GetTankList().Select(g => new { Id = g.Id, Name = g.Code }), "Id", "Name");
            ViewBag.SensorDeviceList = new SelectList(sensorDeviceService.GetSensorDeviceList().Select(g => new { Id = g.Id, Name = g.SerialNumber }), "Id", "Name");

            return View(depotViewModel);
        }

        public ActionResult GetList()
        {
            List<DepotTank> list = depotTankService.GetDepotTankList();

            List<DepotTankViewModel> modelList = new List<DepotTankViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new DepotTankViewModel()
                {
                    Id = item.Id,
                    DepotId = item.DepotId,
                    DepotCode = item.Depot.Code,
                    TankId = item.TankId,
                    TankCode = item.Tank.Code,
                    SensorDeviceId = item.SensorDeviceId,
                    SensorDeviceSN = item.SensorDevice.SerialNumber
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(DepotTank model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                DepotTank depot = null;
                DepotTank oldDepotTank = null;
                if (model.Id == 0)
                {
                    depot = new DepotTank
                    {
                        DepotId = model.DepotId,
                        TankId = model.TankId,
                        SensorDeviceId = model.SensorDeviceId
                    };

                    oldDepotTank = new DepotTank();
                    oldData = new JavaScriptSerializer().Serialize(oldDepotTank);
                    newData = new JavaScriptSerializer().Serialize(depot);
                }
                else
                {
                    depot = genericService.GetList<DepotTank>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDepotTank = genericService.GetList<DepotTank>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new DepotTank()
                    {
                        Id = oldDepotTank.Id,
                        DepotId = model.DepotId,
                        TankId = model.TankId,
                        SensorDeviceId = model.SensorDeviceId
                    });

                    depot.DepotId = model.DepotId;
                    depot.TankId = model.TankId;
                    depot.SensorDeviceId = model.SensorDeviceId;

                    newData = new JavaScriptSerializer().Serialize(new DepotTank()
                    {
                        Id = depot.Id,
                        DepotId = depot.DepotId,
                        TankId = depot.TankId,
                        SensorDeviceId = depot.SensorDeviceId
                    });
                }

                genericService.SaveOrUpdate<DepotTank>(depot, depot.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "DepotTanks",
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

            return RedirectToAction("Index", "DepotTank");
        }
    }
}