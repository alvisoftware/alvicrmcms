using asplcrmcms.Areas.Admin.Models.Request;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MenuController : Controller
    {
        private IMenu _iMenu;
        public MenuController(IMenu iMenu)
        {
            _iMenu = iMenu;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfMenus = await _iMenu.GetAllMenu();
            return View(listOfMenus);
        }

        #region Create
        public async Task<IActionResult> Add()
        {
            FillParentCategory();
            return View(new MenuRequestModel());
        }
        void FillParentCategory()
        {
            var getMenus = _iMenu.GetAllMenu().Result;
            List<SelectListItem> listofmenu = new();
            listofmenu.Add(new SelectListItem() { Text = "Parent Category", Value = "0" });
            foreach (var item in getMenus)
            {
                listofmenu.Add(new SelectListItem() { Text = item.Name, Value = item._id });
            }
            ViewBag.Categories = listofmenu;
        }
        [HttpPost]
        public async Task<IActionResult> Add(MenuRequestModel menuRequestModel, IFormCollection keyValuePairs)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(menuRequestModel.Menu))
            {
                ModelState.AddModelError("Menu", "Enter Menu Name");
            }
            if (!ModelState.IsValid)
            {
                FillParentCategory();
                return View();
            }

            string _sParentId = keyValuePairs["categories"].ToString();
            bool _bModify = false;
            MenuModel menuData = new MenuModel();
            if (_sParentId == "0")
            {
                menuData.Name = menuRequestModel.Menu;
                menuData.Submenus = new List<SubMenuModel>();
                _bModify = await _iMenu.ModifyMenu(menuData);

            }
            else
            {
                var getParentMenuDetail = await _iMenu.GetAllMenu(_sParentId);
                MenuModel existingMenu = getParentMenuDetail.FirstOrDefault();
                _bModify = await _iMenu.ModifyMenu(existingMenu, new SubMenuModel() { Name = menuRequestModel.Menu });
            }
            if (_bModify)
                return RedirectToAction("Index");
            else
            {
                FillParentCategory();
                return View();
            }

        }
        #endregion
    }
}
