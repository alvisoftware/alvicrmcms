using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,HR")]
    public class HolidayMasterController : Controller
    {
        private IHolidayMaster _iHoliday;
        public HolidayMasterController(IHolidayMaster iHoliday)
        {
            _iHoliday = iHoliday;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listofholidays = await _iHoliday.GetAllHolidays();
            return View(listofholidays);
        }

        public async Task<IActionResult> Add()
        {
            return View(new HolidayMasterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(HolidayMasterModel hModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(hModel.Date))
                ModelState.AddModelError("Date", "Enter No. of Days");
           
            await _iHoliday.ModifyHolidayDetails(hModel);
            return RedirectToAction();
        }
    }
}

