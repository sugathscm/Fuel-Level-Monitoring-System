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
    public class DistrictController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public DistrictController()
        {
        }

        public DistrictController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Districts
        public ActionResult Index(int? id)
        {
            District diastolic = new District();
            if (id != null)
            {
                diastolic = genericService.GetList<District>().Where(o => o.Id == id).FirstOrDefault();
            }

            ViewBag.ProvinceList = new SelectList(genericService.GetList<Province>(), "Id", "Name");

            return View(diastolic);
        }

        public ActionResult GetList()
        {
            List<District> list = genericService.GetList<District>();

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
        public ActionResult SaveOrUpdate(District model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                District diastolic = null;
                District oldDistrict = null;
                if (model.Id == 0)
                {
                    diastolic = new District
                    {
                        Name = model.Name,
                        IsActive = true
                    };

                    oldDistrict = new District();
                    oldData = new JavaScriptSerializer().Serialize(oldDistrict);
                    newData = new JavaScriptSerializer().Serialize(diastolic);
                }
                else
                {
                    diastolic = genericService.GetList<District>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDistrict = genericService.GetList<District>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new District()
                    {
                        Id = oldDistrict.Id,
                        Name = oldDistrict.Name,
                        IsActive = oldDistrict.IsActive
                    });

                    diastolic.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    diastolic.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new District()
                    {
                        Id = diastolic.Id,
                        Name = diastolic.Name,
                        IsActive = diastolic.IsActive
                    });
                }

                genericService.SaveOrUpdate<District>(diastolic, diastolic.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Districts",
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


            return RedirectToAction("Index", "District");
        }
    }
}