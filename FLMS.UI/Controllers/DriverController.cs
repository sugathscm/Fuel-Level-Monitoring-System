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
    public class DriverController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public DriverController()
        {
        }

        public DriverController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Drivers
        public ActionResult Index(int? id)
        {
            DriverViewModel itemViewModel = new DriverViewModel();
            if (id != null)
            {
                Driver item = new Driver();

                item = genericService.GetList<Driver>().Where(o => o.Id == id).FirstOrDefault();
                itemViewModel = new DriverViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    Address = item.Address,
                    NICNo = item.NICNo,
                    ContactNo = item.ContactNo,
                    DLNo = item.DLNo
                };
            }

            return View(itemViewModel);
        }

        public ActionResult GetList()
        {
            List<Driver> list = genericService.GetList<Driver>();

            List<DriverViewModel> modelList = new List<DriverViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new DriverViewModel()
                {
                    Id = item.Id,
                    IsActive = item.IsActive,
                    Name = item.Name,
                    Address = item.Address,
                    NICNo = item.NICNo,
                    ContactNo = item.ContactNo,
                    DLNo = item.DLNo
                });
            }

            return Json(new { data = modelList }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult SaveOrUpdate(DriverViewModel model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Driver item = null;
                Driver oldDriver = null;
                if (model.Id == 0)
                {
                    item = new Driver
                    {
                        Name = model.Name,
                        Address = model.Address,
                        NICNo = model.NICNo,
                        ContactNo = model.ContactNo,
                        DLNo = model.DLNo,
                        IsActive = true
                    };

                    oldDriver = new Driver();
                    oldData = new JavaScriptSerializer().Serialize(oldDriver);
                    newData = new JavaScriptSerializer().Serialize(item);
                }
                else
                {
                    item = genericService.GetList<Driver>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldDriver = genericService.GetList<Driver>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Driver()
                    {
                        Id = oldDriver.Id,
                        Name = oldDriver.Name,
                        IsActive = oldDriver.IsActive
                    });

                    item.Name = model.Name;
                    item.Address = model.Address;
                    item.NICNo = model.NICNo;
                    item.ContactNo = model.ContactNo;
                    item.DLNo = model.DLNo;
                    item.IsActive = model.IsActive;

                    newData = new JavaScriptSerializer().Serialize(new Driver()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        IsActive = item.IsActive
                    });
                }

                genericService.SaveOrUpdate<Driver>(item, item.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Drivers",
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

            return RedirectToAction("Index", "Driver");
        }
    }
}