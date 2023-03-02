using System.Security.Claims;
using asplcrmcms.Areas.Admin.Models.Request;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,BDE")]
    public class MailController : Controller
    {
        private IMailSend _iMailSend;
        public MailController(IMailSend iMailSend)
        {
            _iMailSend = iMailSend; 
        }
        public async Task<IActionResult> Index()
        {
            var listOfPages = await _iMailSend.GetAllEmails();
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                var sRole = identity.FindFirst(ClaimTypes.Role).Value;
                var createdBy = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (sRole != "Admin")
                {
                    listOfPages = listOfPages.Where(x => x.sendby == createdBy);
                }
            }
            catch
            {

            }
            return View(listOfPages);
        }

        #region Mail Send
        public async Task<IActionResult> Create()
        {
            return View(new MailSendModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(MailSendModel mailSendModel)
        {
            ModelState.Clear();
            if (string.IsNullOrEmpty(mailSendModel.receiver))
                ModelState.AddModelError("receiver", "Enter emailid");
            if (string.IsNullOrEmpty(mailSendModel.subject))
                ModelState.AddModelError("receiver", "Enter Subject");
            if (string.IsNullOrEmpty(mailSendModel.message))
                ModelState.AddModelError("receiver", "Enter Mail");
            if (!ModelState.IsValid)
            {
                return View();
            }
            var identity = (ClaimsIdentity)User.Identity;
            var createdBy = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            foreach (string mailTo in mailSendModel.receiver.Split(','))
            {
                MailSendModel emailModel = new MailSendModel();
                emailModel.sendby = createdBy;
                emailModel.receiver = mailTo;
                emailModel.message = mailSendModel.message;
                emailModel.subject = mailSendModel.subject;
                await _iMailSend.SendMail(emailModel);
            }

            return View(new MailSendModel());
        }
        #endregion
    }
}
