using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BorBaNetCore.DataModel;
using BorBaNetCore.Services;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BorBaNetCore.Controllers
{
    //[Route("api/[controller]")]
    public class MessageController : Controller
    {
        IMessageManager _msg;
        IUserManager _userManager;
        public MessageController(IMessageManager msg, IUserManager userManager)
        {
            _msg = msg;
            _userManager = userManager;
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
        }

        public IActionResult Index()
        {
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
            return View();
        }
    }
}
