using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    public class CMSPageController : Controller
    {
        private ICMSPage _iCMSPage;
        private IMenu _iMenu;

        public CMSPageController(ICMSPage iCMSPage, IMenu iMenu)
        {
            _iCMSPage = iCMSPage;
            _iMenu = iMenu;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iCMSPage.GetAllPage();
            return View(listOfPages);
        }

        public async Task<IActionResult> Edit(string _sPageId)
        {
            CMSPageModel pageModel = _iCMSPage.GetAllPage(_sPageId).Result.FirstOrDefault();
            if (pageModel == null) return RedirectToAction("Index");
            return View(pageModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CMSPageModel pageModel)
        {
            await _iCMSPage.ModifyPage(pageModel);
            return RedirectToAction("Index");

        }
    }
}
