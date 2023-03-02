using System.Security.Claims;
using asplcrmcms.Areas.Admin.Models.Request;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class CustomerController : Controller
    {
        private ICustomer _iCustomer;
        private IPortfolio _iPortfolio;
        public CustomerController(ICustomer iCustomer, IPortfolio iPortfolio)
        {
            _iCustomer = iCustomer; _iPortfolio = iPortfolio;
        }
        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iCustomer.GetAllCustomers();
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var sRole = identity.FindFirst(ClaimTypes.Role).Value;
                var createdBy = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (sRole != "Admin")
                {
                    listOfPages = listOfPages.Where(x => x.Createdby == createdBy);
                }
            }
            catch
            {
                return null;
            }
            return View(listOfPages);
        }

        #region Create
        public async Task<IActionResult> Add()
        {
            FillPortfolio();
            return View(new CustomerDataModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CustomerDataModel customerDataModel, IFormCollection keyValuePairs)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(customerDataModel.Companydetail.Name))
                ModelState.AddModelError("Companydetail.Name", "Enter company Name");
            if (string.IsNullOrEmpty(customerDataModel.Companydetail.Email))
                ModelState.AddModelError("Companydetail.Email", "Enter E-mail");
            if (string.IsNullOrEmpty(customerDataModel.Companydetail.Phone))
                ModelState.AddModelError("Companydetail.Phone", "Enter Phone");
            if (string.IsNullOrEmpty(customerDataModel.persondetail.Name))
                ModelState.AddModelError("Persondetail.Name", "Enter Person Name");
            if (string.IsNullOrEmpty(customerDataModel.persondetail.Email))
                ModelState.AddModelError("Persondetail.Email", "Enter E-mail");
            if (string.IsNullOrEmpty(customerDataModel.persondetail.Phone))
                ModelState.AddModelError("Persondetail.Phone", "Enter Phone");
            if (!ModelState.IsValid)
            {
                FillPortfolio();
                return View();
            }
            string _sMenuId = keyValuePairs["Suitablefor"].ToString();
            customerDataModel.Suitablefor = _sMenuId;
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                customerDataModel.Createdby = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            catch
            {

            }
            await _iCustomer.ModifyCustomer(customerDataModel);
            return RedirectToAction("Index");

        }
        void FillPortfolio()
        {
            var getPortfolios = _iPortfolio.GetAllPortfolio().Result;
            List<SelectListItem> listofportfolio = new();
            listofportfolio.Add(new SelectListItem() { Text = "NA", Value = "NA" });
            foreach (var item in getPortfolios)
            {
                listofportfolio.Add(new SelectListItem() { Text = item.Title, Value = item.Title });
            }
            ViewBag.Portfolio = listofportfolio;
        }
        #endregion
    }
}
