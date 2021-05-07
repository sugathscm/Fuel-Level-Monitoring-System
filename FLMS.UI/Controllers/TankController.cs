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
    public class TankController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();
        private readonly TankService tankService = new TankService();

        public TankController()
        {
        }

        public TankController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Tanks
        public ActionResult Index(int? id)
        {
            TankViewModel tankViewModel = new TankViewModel();
            if (id != null)
            {
                Tank tank = tankService.GetTankById(id);

                tankViewModel = new TankViewModel()
                {
                    Id = tank.Id,
                    IsActive = tank.IsActive,
                    Code = tank.Code,
                    Volume = tank.Volume,
                    FuelTypeName = tank.FuelType.Name,
                    FuelTypeId = tank.FuelTypeId.Value
                };
            }

            ViewBag.FuelTypeList = new SelectList(genericService.GetList<FuelType>(), "Id", "Name");

            return View(tankViewModel);
        }

        public ActionResult GetList()
        {
            List<Tank> list = tankService.GetTankList();

            List<TankViewModel> modelList = new List<TankViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new TankViewModel() { Id = item.Id, IsActive = item.IsActive, Code = item.Code, Volume = item.Volume, FuelTypeName = item.FuelType.Name });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(Tank model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Tank tank = null;
                Tank oldTank = null;
                if (model.Id == 0)
                {
                    tank = new Tank
                    {
                        Code = model.Code,
                        Volume = model.Volume,
                        FuelTypeId = model.FuelTypeId,
                        IsActive = true
                    };

                    oldTank = new Tank();
                    oldData = new JavaScriptSerializer().Serialize(oldTank);
                    newData = new JavaScriptSerializer().Serialize(tank);
                }
                else
                {
                    tank = genericService.GetList<Tank>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldTank = genericService.GetList<Tank>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Tank()
                    {
                        Id = oldTank.Id,
                        Code = oldTank.Code,
                        IsActive = oldTank.IsActive
                    });

                    tank.Code = model.Code;
                    tank.Volume = model.Volume;
                    tank.FuelTypeId = model.FuelTypeId;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    tank.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Tank()
                    {
                        Id = tank.Id,
                        Code = tank.Code,
                        IsActive = tank.IsActive
                    });
                }

                genericService.SaveOrUpdate<Tank>(tank, tank.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Tanks",
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

            return RedirectToAction("Index", "Tank");
        }
    }
}