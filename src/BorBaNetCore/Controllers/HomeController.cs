
using BorBaNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Globalization;
using System.Threading;

namespace BorBaNetCore.Controllers
{
    public class HomeController : BaseController
    {
        IHtmlLocalizer<HomeController> _localizer;      
        IHttpContextAccessor _ctx;

        public HomeController(IUserManager userManager, IHtmlLocalizer<HomeController> localizer, IHttpContextAccessor ctx) :base(ctx, userManager)
        {          
     
            _localizer = localizer;
            _ctx = ctx;           
        }
        public IActionResult Index([FromHeader(Name = "Accept-Language")]string language)
        {
            string _DefaultCookieName = HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
            ViewBag.CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
