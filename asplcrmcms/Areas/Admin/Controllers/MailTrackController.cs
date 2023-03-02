using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Mvc;

namespace asplcrmcms.Areas.Admin.Controllers
{

    [ApiController]
    public class MailTrackController : ControllerBase
    {
        private IMailSend _iMailSend;
        public MailTrackController(IMailSend iMailSend)
        {
            _iMailSend = iMailSend;
        }
        [Route("api/mailtrack/{id}")]
        public async void mailtrack(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _iMailSend.MailTrack(new MailTrackModel() { emailId = id });
                }
            }
            catch
            {

            }
        }
    }
}
