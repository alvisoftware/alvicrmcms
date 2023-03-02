using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.DBHelper;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class ProjectController : Controller
    {
        private IProject _iCompany;
        private IHostingEnvironment _environment;

        public ProjectController(IProject iCompany, IHostingEnvironment environment)
        {
            _iCompany = iCompany;
            _environment = environment;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iCompany.GetProjectInfo();

            return View(listOfPages);
        }

        public async Task<IActionResult> Add()
        {

            return View(new ProjectModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProjectModel companyInfo)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(companyInfo.Company))
                ModelState.AddModelError("Company", "Enter Company ");
            if (string.IsNullOrEmpty(companyInfo.Name))
                ModelState.AddModelError("Name", "Enter Name");
            if (string.IsNullOrEmpty(companyInfo.Cost))
                ModelState.AddModelError("Address2", "Enter Cost");
            if (string.IsNullOrEmpty(companyInfo.Technology))
                ModelState.AddModelError("Country", "Enter Technology");
            //
            string path = Path.Combine(this._environment.WebRootPath, Constantdata.ProjectDirectoryName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (IFormFile formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                    if (formFile.Name == "Upload")
                    {
                        fileName = fileName + "_cover_" + Path.GetExtension(formFile.FileName);
                        companyInfo.Upload =  Constantdata.ProjectDirectoryName + "/" + fileName.ToLowerInvariant();
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
                    await _iCompany.ModifyProjectInfo(companyInfo);
            return RedirectToAction();

        }

    }
}

