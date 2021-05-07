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
    public class OrganizationController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public OrganizationController()
        {
        }

        public OrganizationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Organizations
        public ActionResult Index(int? id)
        {
            OrganizationViewModel itemViewModel = new OrganizationViewModel();
            if (id != null)
            {
                Organization item = new Organization();

                item = genericService.GetList<Organization>().Where(o => o.Id == id).FirstOrDefault();
                itemViewModel = new OrganizationViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    Address = item.Address,
                    Website = item.Website,
                    ContactDetails = item.ContactDetails
                };
            }

            return View(itemViewModel);
        }

        public ActionResult GetList()
        {
            List<Organization> list = genericService.GetList<Organization>();

            List<OrganizationViewModel> modelList = new List<OrganizationViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new OrganizationViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    Address = item.Address,
                    Website = item.Website,
                    ContactDetails = item.ContactDetails
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(OrganizationViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Organization item = null;
                Organization oldOrganization = null;
                if (model.Id == 0)
                {
                    item = new Organization
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Website = model.Website,
                        ContactDetails = model.ContactDetails,
                        IsActive = true
                    };

                    oldOrganization = new Organization();
                    oldData = new JavaScriptSerializer().Serialize(oldOrganization);
                    newData = new JavaScriptSerializer().Serialize(item);
                }
                else
                {
                    item = genericService.GetList<Organization>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldOrganization = genericService.GetList<Organization>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Organization()
                    {
                        Id = oldOrganization.Id,
                        Name = oldOrganization.Name,
                        IsActive = oldOrganization.IsActive
                    });

                    item.Name = model.Name;
                    item.Address = model.Address;
                    item.Website = model.Website;
                    item.ContactDetails = model.ContactDetails;
                    item.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Organization()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        IsActive = item.IsActive
                    });
                }

                genericService.SaveOrUpdate<Organization>(item, item.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Organizations",
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

            return RedirectToAction("Index", "Organization");
        }
    }
}