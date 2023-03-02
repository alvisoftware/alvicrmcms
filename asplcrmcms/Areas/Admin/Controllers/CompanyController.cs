using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Drawing.Text;
using System.Security.Claims;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class CompanyController : Controller
    {
        private ICompany _iCompany;
        
        public CompanyController(ICompany iCompany)
        {
            _iCompany = iCompany; 
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iCompany.GetCompanyDetails();
            return View(listOfPages);
        }

        public async Task<IActionResult> Add()
        {
           return View(new CompanyDetails());
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(CompanyDetails customerDataModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(customerDataModel.CompanyName))
                ModelState.AddModelError("CompanyName", "Enter company Name");
            if (string.IsNullOrEmpty(customerDataModel.Address))
                ModelState.AddModelError("Address", "Enter Address");
            if (string.IsNullOrEmpty(customerDataModel.Address2))
                ModelState.AddModelError("Address2", "Enter Address2");
            if (string.IsNullOrEmpty(customerDataModel.Country))
                ModelState.AddModelError("Country", "Enter Country Name");
            if (string.IsNullOrEmpty(customerDataModel.City))
                ModelState.AddModelError("City", "Enter City");
            if (string.IsNullOrEmpty(customerDataModel.State))
                ModelState.AddModelError("State", "Enter State");
            if (string.IsNullOrEmpty(customerDataModel.Website))
                ModelState.AddModelError("Website", "Enter Website");
            if (string.IsNullOrEmpty(customerDataModel.Email))
                ModelState.AddModelError("Email", "Enter Email");
            if (string.IsNullOrEmpty(customerDataModel.Contact))
                ModelState.AddModelError("Contact", "Enter Contact");

            await _iCompany.ModifyCompany(customerDataModel);
            return RedirectToAction();

        }
    }
}
