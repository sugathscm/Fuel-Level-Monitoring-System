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
    public class DepotTypeController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public DepotTypeController()
        {
        }

        public DepotTypeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: DepotTypes
        public ActionResult Index(int? id)
        {
            DepotType diastolic = new DepotType();
            if (id != null)
            {
                diastolic = genericService.GetList<DepotType>().Where(o => o.Id == id).FirstOrDefault();
            }
            return View(diastolic);
        }

        public ActionResult GetList()
        {
            List<DepotType> list = genericService.GetList<DepotType>();

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
        public ActionResult SaveOrUpdate(DepotType model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                DepotType diastolic = null;
                DepotType oldDepotType = null;
                if (model.Id == 0)
                {
                    diastolic = new DepotType
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldDepotType = new DepotType();
                    oldData = new JavaScriptSerializer().Serialize(oldDepotType);
                    newData = new JavaScriptSerializer().Serialize(diastolic);
                }
                else
                {
                    diastolic = genericService.GetList<DepotType>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDepotType = genericService.GetList<DepotType>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new DepotType()
                    {
                        Id = oldDepotType.Id,
                        Name = oldDepotType.Name,
                        IsActive = oldDepotType.IsActive
                    });

                    diastolic.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    diastolic.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new DepotType()
                    {
                        Id = diastolic.Id,
                        Name = diastolic.Name,
                        IsActive = diastolic.IsActive
                    });
                }

                genericService.SaveOrUpdate<DepotType>(diastolic, diastolic.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "DepotTypes",
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

            return RedirectToAction("Index", "DepotType");
        }
    }
}