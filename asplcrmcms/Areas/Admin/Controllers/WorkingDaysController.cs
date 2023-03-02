using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class WorkingDaysController : Controller
    {
        private IWorkingDays _iHoliday;
        public WorkingDaysController(IWorkingDays iHoliday)
        {
            _iHoliday = iHoliday;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listofWorkingdays = await _iHoliday.GetWorkingDaysDetails();
            return View(listofWorkingdays);
        }

        public async Task<IActionResult> Add() 
        {
            return View(new WorkingDaysModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(WorkingDaysModel wModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(wModel.Month))
                ModelState.AddModelError("Month", "Enter Month");
            if (string.IsNullOrEmpty(wModel.Year))
                ModelState.AddModelError("Year", "Enter Year");
            if (string.IsNullOrEmpty(wModel.WorkingDays))
                ModelState.AddModelError("Date", "Enter No. of WorkingDays");
           
          
            await _iHoliday.ModifyWorkingDays(wModel);
            return RedirectToAction();
        }
    }
}
