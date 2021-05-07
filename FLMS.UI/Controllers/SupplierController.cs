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
    public class SupplierController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly GenericService genericService = new GenericService();

        public SupplierController()
        {
        }

        public SupplierController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: Suppliers
        public ActionResult Index(int? id)
        {
            SupplierViewModel supplierViewModel = new SupplierViewModel();

            if (id != null)
            {
                Supplier supplier = new Supplier();
                supplier = genericService.GetList<Supplier>().Where(o => o.Id == id).FirstOrDefault();
                supplierViewModel = new SupplierViewModel()
                {
                    Id = supplier.Id,
                    IsActive = supplier.IsActive,
                    Name = supplier.Name,
                    Address = supplier.Address,
                    Website = supplier.Website,
                    ContactDetails = supplier.ContactDetails
                };
            }

            return View(supplierViewModel);
        }


        public ActionResult GetList()
        {
            List<Supplier> list = genericService.GetList<Supplier>();

            List<SupplierViewModel> modelList = new List<SupplierViewModel>();

            foreach (var item in list)
            {
                modelList.Add(new SupplierViewModel() { 
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
        public ActionResult SaveOrUpdate(Supplier model)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                int id = model.Id;
                Supplier supplier = null;
                Supplier oldSupplier = null;
                if (model.Id == 0)
                {
                    supplier = new Supplier
                    {
                        Name = model.Name,
                        Address = model.Address,
                        Website = model.Website,
                        ContactDetails = model.ContactDetails,
                        IsActive = true
                    };

                    oldSupplier = new Supplier();
                    oldData = new JavaScriptSerializer().Serialize(oldSupplier);
                    newData = new JavaScriptSerializer().Serialize(supplier);
                }
                else
                {
                    supplier = genericService.GetList<Supplier>().Where(o => o.Id == model.Id).FirstOrDefault();
                    oldSupplier = genericService.GetList<Supplier>().Where(o => o.Id == model.Id).FirstOrDefault();

                    oldData = new JavaScriptSerializer().Serialize(new Supplier()
                    {
                        Id = oldSupplier.Id,
                        Name = oldSupplier.Name,
                        IsActive = oldSupplier.IsActive
                    });

                    supplier.Name = model.Name;
                    bool Example = Convert.ToBoolean(Request.Form["IsActive.Value"]);
                    supplier.IsActive = model.IsActive;
                    supplier.Address = model.Address;
                    supplier.Website = model.Website;
                    supplier.ContactDetails = model.ContactDetails;

                    newData = new JavaScriptSerializer().Serialize(new Supplier()
                    {
                        Id = supplier.Id,
                        Name = supplier.Name,
                        IsActive = supplier.IsActive
                    });
                }

                genericService.SaveOrUpdate<Supplier>(supplier, supplier.Id);

                //CommonService.SaveDataAudit(new DataAudit()
                //{
                //    Entity = "Suppliers",
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

            return RedirectToAction("Index", "Supplier");
        }
    }
}