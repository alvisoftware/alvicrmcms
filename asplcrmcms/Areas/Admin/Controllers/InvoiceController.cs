using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using Document = iTextSharp.text.Document;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class InvoiceController : Controller
    {
        private IInvoice _iInvoice;
        private ICompany _iCompany;
        private IProject _iProject;
        public InvoiceController(IInvoice iInvoice, ICompany iCompany, IProject iProject)
        {
            _iInvoice = iInvoice;
            _iCompany = iCompany;
            _iProject = iProject;
        }

        [HttpGet]

        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iInvoice.GetCompanyInvoice();

            return View(listOfPages);
        }

        
        public async Task<IActionResult> Add()
        {
            var getCompanies = _iCompany.GetCompanyDetails().Result;
            List<SelectListItem> listofcompanies = new();
            listofcompanies.Add(new SelectListItem() { Text = "Select Company", Value = "0"  });
            foreach (var item in getCompanies)
            {
                listofcompanies.Add(new SelectListItem() { Text = item.CompanyName, Value = item.CompanyName });
            }
            ViewBag.company = listofcompanies;
            //
            var getProjects = _iProject.GetProjectInfo().Result;
            List<SelectListItem> listofproject = new();
            listofproject.Add(new SelectListItem() { Text = "Select Project", Value = "0"  });
            foreach (var item in getProjects)
            {
                listofproject.Add(new SelectListItem() { Text = item.Name, Value = item.Name });
            }
            ViewBag.project = listofproject;
            return View(new CompanyInvoice());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CompanyInvoice compInv, IFormCollection keyValuePairs)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(compInv.CompanyName))
                ModelState.AddModelError("CompanyName", "Enter Company Name");
            if (string.IsNullOrEmpty(compInv.InvoiceNo))
                ModelState.AddModelError("InvoiceNo", "Enter InvoiceNo.");           
            if (string.IsNullOrEmpty(compInv.Total))
                ModelState.AddModelError("Rate", "Enter Total");
            if (string.IsNullOrEmpty(compInv.Tax))
                ModelState.AddModelError("Quantity2", "Enter Tax");
            if (string.IsNullOrEmpty(compInv.FinalAmount))
                ModelState.AddModelError("Rate3", "Enter Final Amount");

           
            List<InvoiceDetail> DetailsList = new List<InvoiceDetail>();
           
            for (int i = 1; i < 4; i++)
            {
                string _sservice1title = keyValuePairs["service" + i.ToString() + "title"].ToString();
                string _sservice1overview = keyValuePairs["service" + i.ToString() + "overview"].ToString();
                string _sservice1titlewithicon = keyValuePairs["service" + i.ToString() + "titlewithicon"].ToString();
                if (!string.IsNullOrEmpty(_sservice1title) && !string.IsNullOrEmpty(_sservice1overview))
                {
                    DetailsList.Add(new InvoiceDetail()
                    {
                        Description = _sservice1title,
                        Quantity = _sservice1overview,
                       // Id = _sservice1title.ToPrimaryKey(),
                        Rate = _sservice1titlewithicon
                    });
                }
            }
            //pdf
            Document document = new Document(PageSize.A4, 36, 36, 36, 36);
            string filepath = "C:\\PDF\\invoiceDetailsss.pdf";
            using (FileStream filesStream = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, filesStream);
                document.Open();
                string para = "To,";
                Paragraph paragraph = new Paragraph(para);
                Paragraph paragraph1 = new Paragraph(compInv.CompanyName);
                Paragraph paragraph2 = new Paragraph(compInv.InvoiceNo);
                document.Add(paragraph);
                document.Add(paragraph1);
                document.Add(paragraph2);
                PdfPTable table = new PdfPTable(3);
                
                table.AddCell("Description");
                table.AddCell("Quantity");
                table.AddCell("Amount");
                foreach (var details in DetailsList)
                {
                    table.AddCell(new PdfPCell(new Phrase(details.Description)) { Border=PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER});
                    table.AddCell(new PdfPCell(new Phrase(details.Quantity)) { Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER } ); 
                    table.AddCell(new PdfPCell(new Phrase(details.Rate + ".00")) { Border = PdfPCell.LEFT_BORDER | PdfPCell.RIGHT_BORDER });
                }
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.SpacingBefore = 10f;
                document.Add(table);
               
                PdfPTable t2 = new PdfPTable(2);

                t2.AddCell("Total");
                t2.AddCell(compInv.Total);
                t2.AddCell("Tax");
                t2.AddCell(compInv.Tax);
                t2.AddCell("Final Amount");
                t2.AddCell(compInv.FinalAmount);
                t2.HorizontalAlignment = Element.ALIGN_LEFT;
               // t2.SpacingBefore = 10f;
                document.Add(t2);
                document.Close();
            }

            //
            compInv.Details = DetailsList;
            await _iInvoice.ModifyInvoice(compInv);
            return RedirectToAction();

        }

    }
}
