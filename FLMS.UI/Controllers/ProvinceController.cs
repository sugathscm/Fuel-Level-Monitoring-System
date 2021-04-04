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
    public class ProvinceController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public ProvinceController()
        {
        }

        public ProvinceController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Provinces
        public ActionResult Index(int? id)
        {
            Province diastolic = new Province();
            if (id != null)
            {
                diastolic = genericService.GetList<Province>().Where(o => o.Id == id).FirstOrDefault();
            }
            return View(diastolic);
        }

        public ActionResult GetList()
        {
            List<Province> list = genericService.GetList<Province>();

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
        public ActionResult SaveOrUpdate(Province model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Province diastolic = null;
                Province oldProvince = null;
                if (model.Id == 0)
                {
                    diastolic = new Province
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldProvince = new Province();
                    oldData = new JavaScriptSerializer().Serialize(oldProvince);
                    newData = new JavaScriptSerializer().Serialize(diastolic);
                }
                else
                {
                    diastolic = genericService.GetList<Province>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldProvince = genericService.GetList<Province>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Province()
                    {
                        Id = oldProvince.Id,
                        Name = oldProvince.Name,
                        IsActive = oldProvince.IsActive
                    });

                    diastolic.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    diastolic.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Province()
                    {
                        Id = diastolic.Id,
                        Name = diastolic.Name,
                        IsActive = diastolic.IsActive
                    });
                }

                genericService.SaveOrUpdate<Province>(diastolic, diastolic.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Provinces",
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

            return RedirectToAction("Index", "Province");
        }
    }
}