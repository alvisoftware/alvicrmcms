using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class ExpenseController : Controller
    {
       
        private IExpense _iExpense;
        public ExpenseController(IExpense iExpense)
        {
            _iExpense = iExpense;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listofExpenses = await _iExpense.GetExpenseDetails();
            return View(listofExpenses);
        }
        public async Task<IActionResult> Add()
        {
            return View(new ExpenseModel());
        }
        [HttpPost]
        public async Task<IActionResult> Add(ExpenseModel expense)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(expense.Name))
                ModelState.AddModelError("Name", "Enter Name");
            if (string.IsNullOrEmpty(expense.Amount))
                ModelState.AddModelError("Amount", "Enter Amount");
            await _iExpense.ModifyExpenseDetails(expense);
            return RedirectToAction();
        }
        
    }
}
