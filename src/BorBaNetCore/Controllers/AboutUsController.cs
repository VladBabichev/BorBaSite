using BorBaNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.CodeAnalysis.Options;
using System;
using System.Threading;

namespace BorBaNetCore.Controllers
{
    public class AboutUs : BaseController
    {
       
        public AboutUs(IUserManager userManager, IHtmlLocalizer<HomeController> localizer, IHttpContextAccessor ctx) : base(ctx, userManager)
        {
          
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            setCurrentCulture();
            return View();
        }      

    }
}
