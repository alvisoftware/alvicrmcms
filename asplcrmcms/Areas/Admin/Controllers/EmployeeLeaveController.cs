using DocumentFormat.OpenXml.ExtendedProperties;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class EmployeeLeaveController : Controller
    {
            private IEmployeeLeave _eLeave;
            private iEmployee _iEmp;    
            public EmployeeLeaveController(IEmployeeLeave iLeave,iEmployee iEmp)
            {
                _eLeave = iLeave;
                _iEmp = iEmp;
            }
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var listofLeaves = await _eLeave.GetLeaveDetails();
                return View(listofLeaves);
            }

            public async Task<IActionResult> Add()
            {
               GetEmployeeName();
               return View(new EmployeeLeaveModel());
            }
            void GetEmployeeName()
            {
            var getDetails = _iEmp.GetEmployeeDetails().Result;
            List<SelectListItem> listofemployees = new();
            listofemployees.Add(new SelectListItem() { Text = "Select Employee", Value = "0" });
            foreach (var item in getDetails)
              {
                listofemployees.Add(new SelectListItem() { Text = item.FullName, Value = item.FullName });
              }
            ViewBag.Leave = listofemployees;
            }

            [HttpPost]
            public async Task<IActionResult> Add(EmployeeLeaveModel eLeave)
            {
                ModelState.Clear();
               
                if (string.IsNullOrEmpty(eLeave.EmployeeName))
                    ModelState.AddModelError("EmployeeLeave", "Employee Leave is required");
                await _eLeave.ModifyLeaveDetails(eLeave);
                return RedirectToAction();
            }
    }
    
}
