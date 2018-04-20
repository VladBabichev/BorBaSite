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
    public class ProductController : BaseController
    {
       
        public ProductController(IUserManager userManager, IHtmlLocalizer<HomeController> localizer, IHttpContextAccessor ctx) : base(ctx, userManager)
        {
          
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            setCurrentCulture();
            return View();
        }

		public IActionResult EQWin()
		{
			setCurrentCulture();
			return PartialView("_EQWin");
		}

		public IActionResult EQWeb()
		{
			setCurrentCulture();
			return PartialView("_EQWeb");
		}

		public IActionResult EQMobile()
		{
			setCurrentCulture();
			return PartialView("_EQMobile");
		}
		public IActionResult Mvp()
		{
			setCurrentCulture();
			return PartialView("_Mvp");
		}

		public IActionResult Aifp()
        {
            return PartialView("_Aifp");
        }
		public IActionResult AifpWeb()
		{
			return PartialView("_AifpWeb");
		}
		public IActionResult PropertyStore()
		{
			setCurrentCulture();
			return PartialView("_PropertyStore");
		}
		public IActionResult Eps()
		{
			setCurrentCulture();
			return PartialView("_Eps");
		}
		public IActionResult Ted()
		{
			setCurrentCulture();
			return PartialView("_Ted");
		}

		public IActionResult H2O()
        {
            setCurrentCulture();
            return PartialView("_H2O");
        }
		public IActionResult Msj()
		{
			setCurrentCulture();
			return PartialView("_Msj");
		}
		public IActionResult MsjMobile()
		{
			setCurrentCulture();
			return PartialView("_MsjMobile");
		}
		public IActionResult SmartHouse()
        {
            setCurrentCulture();
            return PartialView("_SmartHouse");
        }
		public IActionResult Checkpoint()
		{
			setCurrentCulture();
			return PartialView("_Checkpoint");
		}
		public IActionResult Kdt()
		{
			setCurrentCulture();
			return PartialView("_Kdt");
		}
		public IActionResult Zsh()
		{
			setCurrentCulture();
			return PartialView("_Zsh");
		}

	}
}
