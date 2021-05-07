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
    public class FuelTypeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public FuelTypeController()
        {
        }

        public FuelTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: FuelTypes
        public ActionResult Index(int? id)
        {
            FuelType diastolic = new FuelType();
            if (id != null)
            {
                diastolic = genericService.GetList<FuelType>().Where(o => o.Id == id).FirstOrDefault();
            }
            return View(diastolic);
        }

        public ActionResult GetList()
        {
            List<FuelType> list = genericService.GetList<FuelType>();

            List<BaseViewModel> modelList = new List<BaseViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new BaseViewModel() { Id = item.Id, IsActive = item.IsActive, Name = item.Name });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(FuelType model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                FuelType diastolic = null;
                FuelType oldFuelType = null;
                if (model.Id == 0)
                {
                    diastolic = new FuelType
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldFuelType = new FuelType();
                    oldData = new JavaScriptSerializer().Serialize(oldFuelType);
                    newData = new JavaScriptSerializer().Serialize(diastolic);
                }
                else
                {
                    diastolic = genericService.GetList<FuelType>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldFuelType = genericService.GetList<FuelType>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new FuelType()
                    {
                        Id = oldFuelType.Id,
                        Name = oldFuelType.Name,
                        IsActive = oldFuelType.IsActive
                    });

                    diastolic.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    diastolic.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new FuelType()
                    {
                        Id = diastolic.Id,
                        Name = diastolic.Name,
                        IsActive = diastolic.IsActive
                    });
                }

                genericService.SaveOrUpdate<FuelType>(diastolic, diastolic.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "FuelTypes",
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

            return RedirectToAction("Index", "FuelType");
        }
    }
}