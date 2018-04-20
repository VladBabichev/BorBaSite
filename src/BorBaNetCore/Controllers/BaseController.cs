using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.RegularExpressions;
using BorBaNetCore.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace BorBaNetCore.Controllers
{
    public class BaseController : Controller
    {
        protected string _DefaultCookieName;
        protected IUserManager _userManager;
        IHttpContextAccessor _ctx;
        public BaseController(IHttpContextAccessor ctx,IUserManager userManager)
        {
            _ctx = ctx;
            ViewBag.CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
            _DefaultCookieName = ctx.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            _userManager = userManager;
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
        }
        protected CultureInfo getUserCultureInfoFromHeader()
        {
            string userLanguages = Request.Headers["Accept-Language"];
            CultureInfo ci = !string.IsNullOrEmpty(userLanguages)
                ? new CultureInfo(userLanguages[0])
                : CultureInfo.InvariantCulture;

            return ci;
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {           
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            ViewBag.CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
            return LocalRedirect(returnUrl);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers.ContainsKey("User-Agent") &&
                Regex.IsMatch(context.HttpContext.Request.Headers["User-Agent"].FirstOrDefault(), "MSIE 8.0"))
            {
               // context.Result = Content("Internet Explorer 8.0 не поддерживается");
            }
            base.OnActionExecuting(context);
        }

        protected void setCurrentCulture()
        {
          
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
            ViewBag.CurrentCulture = Thread.CurrentThread.CurrentCulture.Name;
        }

        protected class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }
    }
}