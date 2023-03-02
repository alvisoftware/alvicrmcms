using DocumentFormat.OpenXml.Office.CustomUI;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles="Admin,BDE")]
    public class PaymentController : Controller
    {
        private IPayment _iPayment;
        private IInvoice _iInvoice;
        public PaymentController(IPayment iPayment,IInvoice iInvoice)
        {
            _iPayment = iPayment;
            _iInvoice= iInvoice;
        }
        [HttpGet]
        public async Task<IActionResult> Index() 
        {
            var listofPayments = await _iPayment.GetPaymentDetails();
            return View(listofPayments);
        }
        public async Task<IActionResult> Add()
        {
            var getDetails = _iInvoice.GetCompanyInvoice().Result;
            List<SelectListItem>listItems= new ();
            listItems.Add(new SelectListItem() { Text = "Select", Value = "0" });
            foreach(var item in getDetails)
            {
                listItems.Add(new SelectListItem() {Text= item.ProjectName+"-"+item.InvoiceNo+"-"+item.FinalAmount, Value=item.ProjectName + "-" + item.InvoiceNo + "-" + item.FinalAmount });
            }
            ViewBag.project = listItems;
            return View(new PaymentModel());
        }
        [HttpPost]
        public async Task<IActionResult> Add(PaymentModel payment)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(payment.Project))
                ModelState.AddModelError("Project", "Enter Project");
            if (string.IsNullOrEmpty(payment.Amount))
                ModelState.AddModelError("Amount", "Enter Amount");
            if (string.IsNullOrEmpty(payment.TDS))
                ModelState.AddModelError("TDS", "Enter TDS");
            if (string.IsNullOrEmpty(payment.Remarks))
                ModelState.AddModelError("Remarks", "Enter Remarks");
            if (string.IsNullOrEmpty(payment.RefNo))
                ModelState.AddModelError("RefNo", "Enter RefNo");
            await _iPayment.ModifyPaymentDetails(payment);
            return RedirectToAction();
        }
    }
}
