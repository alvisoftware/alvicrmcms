using Infrastructure.Interface;
using Infrastructure.Model;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asplcrmcms.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        
        private IAlviUser _iAlviUser;
        public HomeController(IAlviUser iAlviUser)
        {
            _iAlviUser = iAlviUser;
        }
        public IActionResult Index()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserModel _userModel)
        {
            ModelState.Clear();
            if (!string.IsNullOrEmpty(_userModel.Username))
                ModelState.AddModelError("Username", "Enter Username");
            if (!string.IsNullOrEmpty(_userModel.Pwd))
                ModelState.AddModelError("Pwd", "Enter Password");

            UserModel userModel = await _iAlviUser.LoggedIn(_userModel.Username);
            if (userModel != null)
            {
                if (!userModel.Isactive)
                    ModelState.AddModelError("Username", "User is not activated!");

                if (userModel.Pwd == CommonExtension.ComputeSha256Hash(_userModel.Pwd))
                {
                    var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userModel.Fullname),
                    new Claim(ClaimTypes.PrimarySid, userModel._id),
                    new Claim(ClaimTypes.Role, userModel.Role),
                    new Claim(ClaimTypes.NameIdentifier, userModel.Username)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);
                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    return View();

                }

            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            var AuthenticationManager = HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}
