using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BorBaNetCore.Services;
using BorBaNetCore.Extensions;
using BorBaNetCore.DataModel;
using BorBaNetCore.Services.Impl;
using BorBaNetCore.Models;
using Microsoft.AspNetCore.Http;
using BorBaNetCore.Models.AccountViewModels;

namespace BorBaNetCore.Controllers
{
	public class AccountController : Controller
	{
		private readonly ILogger _logger;
		private IUserManager _manager;
        private DbContext _BorBaEntities;
        public AccountController(IUserManager manager, ILoggerFactory loggerFactory, DbContext BorBaEntities)
		{
			_manager = manager;
			_logger = loggerFactory.CreateLogger<AccountController>();
            _BorBaEntities = BorBaEntities;
        }

		//
		// GET: /Account/Login
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			ViewData[Constants.Route.RETURN_URL] = returnUrl;
            LoginViewModel model = new LoginViewModel() { Email = "guest", Password = "guest", RememberMe = true };

            return View(model);
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(Models.AccountViewModels.LoginViewModel model, string returnUrl = null)
		{
          //CreateUser();
          // to create admin on first call
       
            ViewData[Constants.Route.RETURN_URL] = returnUrl;
			if (ModelState.IsValid)
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var admin = await _manager.Login(model.Email, model.Password);
				if (admin != null && (bool)admin.IsActive)
				{
					_logger.LogInformation(1, "User logged in:" + model.Email);
					_manager.SetCurrentUser(HttpContext, admin.ToCurrentUser());

					// If remember me is set, get a remember token and set it to response cookie
					if (model.RememberMe)
					{
						var options = new CookieOptions()
						{
							Expires = System.DateTime.Now.AddMonths(1)
						};
						Response.Cookies.Append(Constants.Cookie.REMEMBER_ME, await _manager.GetToken(admin.UserId), options);
					}
					else
						Response.Cookies.Delete(Constants.Cookie.REMEMBER_ME);

					return RedirectToLocal(returnUrl);
				}
				else
				{
                    //UserManager quest =await _manager.GetByName("guest");
                    //if (quest == null)
                        admin = await _manager.Create("guest", "guest");

                    _manager.SetCurrentUser(HttpContext, admin.ToCurrentUser());
                    //ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //return View(model);
                    return RedirectToLocal(returnUrl);
                }
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Logout
		[HttpGet]
		public IActionResult Logout()
		{
			Response.Cookies.Delete(Constants.Cookie.REMEMBER_ME);
			HttpContext.Session.Clear();
			ViewData[Constants.Route.RETURN_URL] = null;
			return View("Login");
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

        public void CreateUser()
        {
            using (BorBaContext db = new BorBaContext())
            {
                UserManager manager = new UserManager(db);
                Users u = new Users()
                {
                    UserName = "admin",                   
                    IsActive = true,
                    IsSystem = true
                };
                u.UserRoles.Add(new UserRoles()
                {
                    User = u,
                    Role = new Roles()
                    {
                        Abbrev = "Admin",
                        Name = "Admin",
                    }
                });
                Task t = manager.Create(u, "admin");
                t.Wait();
            }
        }
    }
}
