using DocumentFormat.OpenXml.Office.CustomUI;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private IDashboard _iDashboard;
        private IInvoice _iInvoice;
        private IExpense _iExpense;
        private IProject _iProject;
        public DashboardController(IDashboard iDashboard, IInvoice iInvoice, IExpense iExpence, IProject iProject)
        {
            _iDashboard = iDashboard;
            _iInvoice = iInvoice;
            _iExpense = iExpence;
            _iProject = iProject;
            
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var getProjects = _iProject.GetProjectInfo().Result;
            List<SelectListItem> listofproject = new();
            foreach (var item in getProjects)
            {
                listofproject.Add(new SelectListItem() { Value = item.Name });
            }
            ViewBag.Project = listofproject.Count();
           
            var listofRevenue = _iInvoice.GetCompanyInvoice().Result;
            List<SelectListItem> listitem=new List<SelectListItem>();
            foreach(var item in listofRevenue)
            {
                listitem.Add(new SelectListItem { Value=item.FinalAmount });
            }
            ViewBag.Revenue = listitem.Sum(x=>Convert.ToInt16(x.Value));
            
            var listofExpense = _iExpense.GetExpenseDetails().Result;
            List<SelectListItem> listitem2 = new List<SelectListItem>();
            foreach (var item2 in listofExpense)
            {
                listitem2.Add(new SelectListItem { Value = item2.Amount });
            }
            ViewBag.Expense = listitem2.Sum(x => Convert.ToInt16(x.Value));
            return View();
        }
    }
}
