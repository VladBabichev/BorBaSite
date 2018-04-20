
using Microsoft.AspNetCore.Mvc;
using BorBaNetCore.DataModel;
using BorBaNetCore.Services;
using BorBaNetCore.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BorBaNetCore.Controllers.Api
{
    //[Route("api/[controller]")]
    public class MessageController1 : BaseController
    {
        IMessageManager _msg;     
      
        public MessageController1(IMessageManager msg, IUserManager userManager, IHttpContextAccessor ctx) :base(ctx, userManager)
        {
           
            _msg = msg;       
       
        }

        public IActionResult Index()
        {
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
            return View();
        }

       
        [HttpGet]
        public  JsonResult SendMessage(string message)
        {
            //await _notificationsMessageHandler.SendMessageToAllAsync(message);
            var rd = HttpContext.GetRouteData();
            return  Json(message);
        }

       

    }
}
