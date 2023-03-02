using Infrastructure.DBHelper;
using Infrastructure.Interface;
using Infrastructure.Model;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        private IPage _iPage;
        private IMenu _iMenu;
        private IHostingEnvironment _environment;
        private IPortfolio _iPortfolio;

        public PageController(IPage iPage, IMenu iMenu, IHostingEnvironment environment, IPortfolio iPortfolio)
        {
            _iPage = iPage;
            _iMenu = iMenu;
            _environment = environment;
            _iPortfolio = iPortfolio;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iPage.GetAllPage();
            return View(listOfPages);
        }

        #region Create
        public async Task<IActionResult> Add()
        {
            FillParentCategory();
            FillPortfolio();
            return View(new PageModel());

        }
        [HttpPost]
        public async Task<IActionResult> Add(PageModel pageModel, IFormCollection keyValuePairs)
        {
            string _pContent = keyValuePairs["pagecontent"].ToString();
            ModelState.Clear();
            if (string.IsNullOrEmpty(pageModel.Pageurl))
                ModelState.AddModelError("Pageurl", "Enter URL");
            if (string.IsNullOrEmpty(pageModel.PageName))
                ModelState.AddModelError("PageName", "Enter Page Name");
            if (string.IsNullOrEmpty(pageModel.Pagetitle))
                ModelState.AddModelError("Pagetitle", "Enter Page Title");
            if (string.IsNullOrEmpty(pageModel.Pagekeywords))
                ModelState.AddModelError("Pagekeywords", "Enter SEO Page Keywords");
            if (string.IsNullOrEmpty(pageModel.Pagecontent))
                ModelState.AddModelError("Pagecontent", "Enter Page Content");
            if (string.IsNullOrEmpty(pageModel.Pagedescription))
                ModelState.AddModelError("Pagedescription", "Enter SEO Page Description");

            if (!ModelState.IsValid)
            {
                FillParentCategory();
                FillPortfolio();
                return View();
            }
            string path = Path.Combine(this._environment.WebRootPath, Constantdata.PageBackgroundImageDirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (IFormFile formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    if (formFile.Name == "backgrimg")
                    {
                        fileName = pageModel.PageName.ToLowerStringWithoutSpace() + Path.GetExtension(formFile.FileName);
                        pageModel.Backimg = Constantdata.PageBackgroundImageDirectoryName + "/" + fileName.ToLowerInvariant();
                    }
                    using (var inputstream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(inputstream);
                        byte[] array = new byte[inputstream.Length];
                        inputstream.Seek(0, SeekOrigin.Begin);
                        inputstream.Read(array, 0, array.Length);
                    }
                }

            }
            string _sMenuId = keyValuePairs["categories"].ToString();

            #region For Portfolio
            string _sPortfolioIds = keyValuePairs["portfolios"].ToString();
            if (!string.IsNullOrEmpty(_sPortfolioIds))
            {
                pageModel.Pagerelatedworks = _sPortfolioIds;
            }
            #endregion

            #region Services
            List<Servicedescription> servicedescriptionList = new List<Servicedescription>();
            for (int i = 1; i < 7; i++)
            {
                string _sservice1title = keyValuePairs["service" + i.ToString() + "title"].ToString();
                string _sservice1overview = keyValuePairs["service" + i.ToString() + "overview"].ToString();
                string _sservice1titlewithicon = keyValuePairs["service" + i.ToString() + "titlewithicon"].ToString();
                if (!string.IsNullOrEmpty(_sservice1title) && !string.IsNullOrEmpty(_sservice1overview))
                {
                    servicedescriptionList.Add(new Servicedescription()
                    {
                        Title = _sservice1title,
                        Description = _sservice1overview,
                        _id = _sservice1title.ToPrimaryKey(),
                        Titlewithicon = _sservice1titlewithicon
                    });
                }
            }

            pageModel.Serviceoffer = servicedescriptionList;
            pageModel.IsPublish = false;

            #endregion

            pageModel.Menuid = _sMenuId;
            await _iPage.ModifyPage(pageModel);
            return RedirectToAction("Index");

        }
        void FillParentCategory(string? _sSelectedId = null)
        {
            var getMenus = _iMenu.GetAllMenu().Result;
            List<SelectListItem> listofmenu = new();
            foreach (var item in getMenus)
            {
                bool _bIsSelected = !string.IsNullOrEmpty(_sSelectedId) && item._id == _sSelectedId ? true : false;
                listofmenu.Add(new SelectListItem() { Selected = _bIsSelected, Text = item.Name, Value = item._id, Disabled = item.Submenus.Count() > 0 ? true : false });
                if (item.Submenus.Count() > 0)
                {
                    foreach (var submenu in item.Submenus)
                    {
                        _bIsSelected = !string.IsNullOrEmpty(_sSelectedId) && submenu._id == _sSelectedId ? true : false;
                        listofmenu.Add(new SelectListItem() { Text = submenu.Name, Value = submenu._id, Selected = _bIsSelected, });
                    }
                }
            }
            ViewBag.Categories = listofmenu;
        }
        void FillPortfolio()
        {
            var getPortfolios = _iPortfolio.GetAllPortfolio().Result;
            List<SelectListItem> listofportfolio = new();
            foreach (var item in getPortfolios)
            {
                listofportfolio.Add(new SelectListItem() { Text = item.Title, Value = item._id });
            }
            ViewBag.Portfolio = listofportfolio;
        }
        #endregion


        #region Edit
        public async Task<IActionResult> Edit(string _sPageId)
        {
            PageModel pageModel = _iPage.GetAllPage(_sPageId).Result.FirstOrDefault();
            if (pageModel == null) return RedirectToAction("Index");
            FillPortfolio();
            int i = 1;
            foreach(var service in pageModel.Serviceoffer)
            {

                ViewBag.serviceTitle1 = service.Title;
                ViewBag.serviceoverview1 = service.Description;
                ViewBag.serviceTitlewithicon1 = service.Titlewithicon;
                i = i + 1;
            }


            FillParentCategory(pageModel.Menuid);
            return View(pageModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(PageModel pageModel, IFormCollection keyValuePairs)
        {
            string _pContent = keyValuePairs["pagecontent"].ToString();
            ModelState.Clear();
            if (string.IsNullOrEmpty(pageModel.Pageurl))
                ModelState.AddModelError("Pageurl", "Enter URL");
            if (string.IsNullOrEmpty(pageModel.PageName))
                ModelState.AddModelError("PageName", "Enter Page Name");
            if (string.IsNullOrEmpty(pageModel.Pagetitle))
                ModelState.AddModelError("Pagetitle", "Enter Page Title");
            if (string.IsNullOrEmpty(pageModel.Pagekeywords))
                ModelState.AddModelError("Pagekeywords", "Enter SEO Page Keywords");
            if (string.IsNullOrEmpty(pageModel.Pagecontent))
                ModelState.AddModelError("Pagecontent", "Enter Page Content");
            if (string.IsNullOrEmpty(pageModel.Pagedescription))
                ModelState.AddModelError("Pagedescription", "Enter SEO Page Description");

            if (!ModelState.IsValid)
            {
                FillParentCategory();
                FillPortfolio();
                return View();
            }
            string path = Path.Combine(this._environment.WebRootPath, Constantdata.PageBackgroundImageDirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (IFormFile formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    if (formFile.Name == "backgrimg")
                    {
                        fileName = pageModel.PageName.ToLowerStringWithoutSpace() + Path.GetExtension(formFile.FileName);
                        pageModel.Backimg = Constantdata.PageBackgroundImageDirectoryName + "/" + fileName.ToLowerInvariant();
                    }
                    using (var inputstream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(inputstream);
                        byte[] array = new byte[inputstream.Length];
                        inputstream.Seek(0, SeekOrigin.Begin);
                        inputstream.Read(array, 0, array.Length);
                    }
                }

            }
            string _sMenuId = keyValuePairs["categories"].ToString();

            #region For Portfolio
            string _sPortfolioIds = keyValuePairs["portfolios"].ToString();
            if (!string.IsNullOrEmpty(_sPortfolioIds))
            {
                pageModel.Pagerelatedworks = _sPortfolioIds;
            }
            #endregion

            #region Services
            List<Servicedescription> servicedescriptionList = new List<Servicedescription>();
            for (int i = 1; i < 7; i++)
            {
                string _sservice1title = keyValuePairs["service" + i.ToString() + "title"].ToString();
                string _sservice1overview = keyValuePairs["service" + i.ToString() + "overview"].ToString();
                string _sservice1titlewithicon = keyValuePairs["service" + i.ToString() + "titlewithicon"].ToString();
                if (!string.IsNullOrEmpty(_sservice1title) && !string.IsNullOrEmpty(_sservice1overview))
                {
                    servicedescriptionList.Add(new Servicedescription()
                    {
                        Title = _sservice1title,
                        Description = _sservice1overview,
                        _id = _sservice1title.ToPrimaryKey(),
                        Titlewithicon = _sservice1titlewithicon
                    });
                }
            }

            pageModel.Serviceoffer = servicedescriptionList;
            pageModel.IsPublish = false;

            #endregion

            pageModel.Menuid = _sMenuId;
            await _iPage.ModifyPage(pageModel);
            return RedirectToAction("Index");

        }
        #endregion
    }
}
