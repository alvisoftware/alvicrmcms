using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class EmployeeController : Controller
    {
        private iEmployee _iEmployee; 

        public EmployeeController(iEmployee iEmployee)
        {
            _iEmployee = iEmployee;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iEmployee.GetEmployeeDetails();

            return View(listOfPages);
        }
        public async Task<IActionResult> Add()
        {

            return View(new EmployeeModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmployeeModel employeeModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(employeeModel.FullName))
                ModelState.AddModelError("FullName", "Enter Full Name");
            if (string.IsNullOrEmpty(employeeModel.Department))
                ModelState.AddModelError("Department", "Enter Department");
            if (string.IsNullOrEmpty(employeeModel.PanNo))
                ModelState.AddModelError("PanNo", "Enter Pan No");
            if (string.IsNullOrEmpty(employeeModel.AdhaarNo))
                ModelState.AddModelError("AdhaarNo", "Enter AdhaarNo.");
            if (string.IsNullOrEmpty(employeeModel.JobType))
                ModelState.AddModelError("JobType", "Enter JobType");
            if (string.IsNullOrEmpty(employeeModel.JobTitle))
                ModelState.AddModelError("JobTitle", "Enter JobTitle");
            await _iEmployee.ModifyEmployee(employeeModel);
            return RedirectToAction();

        }
    }
}
