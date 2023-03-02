using Infrastructure.DBHelper;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PortfolioController : Controller
    {
        private IPortfolio _iPortfolio;
        private IHostingEnvironment _environment;
        public PortfolioController(IPortfolio iPortfolio, IHostingEnvironment environment)
        {
            _iPortfolio = iPortfolio;
            _environment = environment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listofPortfolio = await _iPortfolio.GetAllPortfolio();
            return View(listofPortfolio);
        }

        #region Create
        public async Task<IActionResult> Add()
        {
            return View(new PortfolioModel());

        }
        [HttpPost]
        public async Task<IActionResult> Add(PortfolioModel portfolioModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(portfolioModel.Pageurl))
                ModelState.AddModelError("Pageurl", "Enter URL");
            if (string.IsNullOrEmpty(portfolioModel.Title))
                ModelState.AddModelError("Title", "Enter Title");
            if (string.IsNullOrEmpty(portfolioModel.Technologies))
                ModelState.AddModelError("Technologies", "Enter Technologies");
            if (string.IsNullOrEmpty(portfolioModel.Pagetitle))
                ModelState.AddModelError("Pagetitle", "Enter Page Title");
            if (string.IsNullOrEmpty(portfolioModel.Pagekeywords))
                ModelState.AddModelError("Pagekeywords", "Enter SEO Page Keywords");
            if (string.IsNullOrEmpty(portfolioModel.Pagedescription))
                ModelState.AddModelError("Pagedescription", "Enter SEO Page Description");

            if (!ModelState.IsValid)
            {
                return View();
            }
            string path = Path.Combine(this._environment.WebRootPath, Constantdata.PortfolioDirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (IFormFile formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    if (formFile.Name == "thumbfile")
                    {
                        fileName = fileName + "_cover_" + Path.GetExtension(formFile.FileName);
                        portfolioModel.Coverimg = Constantdata.PortfolioDirectoryName + "/" + fileName.ToLowerInvariant();
                    }
                    else if (formFile.Name == "portfoliofile")
                    {
                        fileName = fileName + "_portfolio_" + Path.GetExtension(formFile.FileName);
                        portfolioModel.Portfolioimg = Constantdata.PortfolioDirectoryName + "/" + fileName.ToLowerInvariant();
                    }
                    else if (formFile.Name == "bgfile")
                    {
                        fileName = fileName + "_bg_" + Path.GetExtension(formFile.FileName);
                        portfolioModel.Bgimg = Constantdata.PortfolioDirectoryName + "/" + fileName.ToLowerInvariant();
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
            await _iPortfolio.ModifyPortfolio(portfolioModel);
            return RedirectToAction("Index");

        }

        #endregion


        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(String _sPortfolioId)
        {
            if (!string.IsNullOrEmpty(_sPortfolioId))
            {
                await _iPortfolio.DeletePortfolio(_sPortfolioId);
            }
            return RedirectToAction("Index");
        }


        #endregion
    }
}
