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
        private readonly DistrictService districtService = new DistrictService();

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
            District district = new District();
            if (id != null)
            {
                district = districtService.GetDistrictById(id);
            }

            ViewBag.ProvinceList = new SelectList(genericService.GetList<Province>(), "Id", "Name");

            return View(district);
        }

        public ActionResult GetList()
        {
            List<District> list = districtService.GetDistrictList();

            List<DistrictViewModel> modelList = new List<DistrictViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new DistrictViewModel() { Id = item.Id, 
                    IsActive = item.IsActive, 
                    Name = item.Name, 
                    ProvinceName = item.Province.Name
                });
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
                District district = null;
                District oldDistrict = null;
                if (model.Id == 0)
                {
                    district = new District
                    {
                        Name = model.Name,
                        IsActive = true,
                        ProvinceId = model.ProvinceId
                    };

                    oldDistrict = new District();
                    oldData = new JavaScriptSerializer().Serialize(oldDistrict);
                    newData = new JavaScriptSerializer().Serialize(district);
                }
                else
                {
                    district = genericService.GetList<District>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDistrict = genericService.GetList<District>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new District()
                    {
                        Id = oldDistrict.Id,
                        Name = oldDistrict.Name,
                        IsActive = oldDistrict.IsActive
                    });

                    district.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    district.IsActive = model.IsActive;
                    district.ProvinceId = model.ProvinceId;

                    newData = new JavaScriptSerializer().Serialize(new District()
                    {
                        Id = district.Id,
                        Name = district.Name,
                        IsActive = district.IsActive
                    });
                }

                genericService.SaveOrUpdate<District>(district, district.Id);

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