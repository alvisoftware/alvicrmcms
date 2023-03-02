using asplcrmcms.Areas.Admin.Models.Request;
using Infrastructure.Interface;
using Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace asplcrmcms.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private IAlviUser _iAlviUser;
        public UserController(IAlviUser iAlviuser)
        {
            _iAlviUser = iAlviuser;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfUsers = await _iAlviUser.Getallusers();
            return View(listOfUsers);
        }

        public async Task<IActionResult> Add()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserModel userModel)
        {
            await _iAlviUser.Adduser(userModel);
            return RedirectToAction("Index");
        }
    }
}
