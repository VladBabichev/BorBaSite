
using BorBaNetCore.Services;
using Microsoft.AspNetCore.Mvc;
using BorBaNetCore.DataModel;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using BorBaNetCore.Classes;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace BorBaNetCore.Controllers
{
    public class ContactUsController : BaseController
    {
        IMessageManager _msg;
        private readonly IEmailSender _emailSender;
        private MessagesConfig _messagesConfig { get; }
        private IStringLocalizer<SharedResource> _localizer;

        public ContactUsController(IMessageManager msg, IUserManager userManager, IStringLocalizer<SharedResource> localizer, IHttpContextAccessor ctx, IEmailSender emailSender, IOptions<MessagesConfig> msgConfig) : base(ctx, userManager)
        {
            _msg = msg;
            _emailSender = emailSender;
            _messagesConfig = msgConfig.Value;
            _localizer = localizer;
            ViewBag.successMessage = null;
            ViewBag.ErrInfo = null;
            setCurrentCulture();
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            setCurrentCulture();
            return View();
        }
        public async Task<IActionResult> Save(Messages result)
        {
            setCurrentCulture();
            bool isSendToDataBase = false;
            bool isSendToMaile = false;
            bool isSendToRemoteDataBase = false;
            ViewData[Constants.Session.CURRENT_USER] = _userManager.CurrentUser;
            if (ModelState.IsValid && result.Email != null && result.Name != null && result.Subject != null && result.Text != null)
            {
                resend:
                try
                {        
                    
                    if (!isSendToMaile && _messagesConfig.SmtpEnabled)
                    {
                        isSendToMaile = true;
                        await sendEmail(result);
                    }                       

                    if (!isSendToDataBase && _messagesConfig.DataBaseEnabled)
                    {
                        isSendToDataBase = true;
                        if (await saveToDataBase(result))
                            ViewBag.Messages = await _msg.List();                       
                    }

                    if (_messagesConfig.RemoteDataBaseEnabled && !isSendToRemoteDataBase)
                    {
                        isSendToRemoteDataBase = true;
                        if (invokeRemoteDataBase(result) && _messagesConfig.DataBaseEnabled)
                            ViewBag.Messages = await _msg.List();
                    }
                      

                    if (!_messagesConfig.SmtpEnabled && !_messagesConfig.DataBaseEnabled && !isSendToRemoteDataBase)
                    {
                        ViewBag.successMessage = "err";
                        ViewBag.ErrInfo = _localizer["ErrSendEmail"];
                    }                  

                }
                catch (Exception ex)
                {
                    ViewBag.successMessage = "err";
                    if (ex.InnerException != null)
                        ViewBag.ErrInfo = ex.InnerException.Message;
                    else
                        ViewBag.ErrInfo = ex.Message;
                }
                if (!isSendToDataBase && _messagesConfig.DataBaseEnabled)
                    goto resend;
            }
          
            return View("Index");
        }

        //
        // GET: 
        [HttpGet]
        public async Task<IActionResult> List(string name)
        {
            SearchResult<Messages> result=new SearchResult<Messages> ();
            try
            {
                result = await _msg.Search(name);
                ViewBag.Messages = result;
            }
            catch (Exception ex)
            {
                ViewBag.successMessage = "err";
                if (ex.InnerException != null)
                    ViewBag.ErrInfo = ex.InnerException.Message;
                else
                    ViewBag.ErrInfo = ex.Message;
            }
            setCurrentCulture();
            return View(result);
        }

        [HttpPost]
        public JsonResult GetSTL(string model)
        {
            Task<List<Messages>> result = _msg.List();
            //List<Messages> result2 = result.Result;
            return Json(result);
        }

        #region "Api"     
        [HttpPost]
        public async Task SaveToDataBaseFromWebServer([FromBody]Messages result)
        {
            await saveToDataBase(result);
        }

        #endregion


        #region  private methods

        private async Task sendEmail(Messages result)
        {
            bool first = true;
            second:
            try
            {
                if (first)
                    await _emailSender.SendEmailAsync(result.Email, result.Subject, result.Text, result.Name);
                else
                    await _emailSender.SendEmailAsync(result);

                ViewBag.successMessage = "succes";
            }
            catch (Exception ex)
            {
                ViewBag.successMessage = "err";
                if (ex.InnerException != null)
                    ViewBag.ErrInfo = ex.InnerException.Message;
                else
                    ViewBag.ErrInfo = ex.Message;
            }
            if (first && ViewBag.successMessage == "err")
            {
                first = false;
                goto second;
            }
        }

        private async Task<bool> saveToDataBase(Messages msg)
        {
            bool result = false;
            try
            {
                Messages mmsg = new Messages()
                {
                    Email = msg.Email,
                    Name = msg.Name,
                    Subject = msg.Subject,
                    Text = msg.Text,
                    RegDate = DateTime.Now
                };
                //await _msg.Save(msg);
                int id  = await _msg.Save_sp(msg);                
                ViewBag.successMessage = "succes";
                result = true;
            }
            catch (Exception ex)
            {
                ViewBag.successMessage = "err";
                ViewBag.ErrInfo = ex.Message;
                result = false;
            }
            return result;
        }
     
        private bool invokeRemoteDataBase(Messages msg)
        {           
            bool result = false;
            try
            {
                HttpClient client = new HttpClient();              
                //string stringData = JsonConvert.SerializeObject(msg);
                //HttpContent contentData = new StringContent(stringData, System.Text.Encoding.UTF8,"application/json");          
                HttpResponseMessage response = client.PostAsync(_messagesConfig.RemoteDataBaseURL, new JsonContent(msg)).Result;
               string resultMsg = response.Content.ReadAsStringAsync().Result;
              if (string.IsNullOrEmpty(resultMsg))
                {
                    ViewBag.successMessage = "succes";
                    result = true;
                }
                else
                {
                    ViewBag.successMessage = resultMsg;
                }               
            }
            catch (Exception ex)
            {
                ViewBag.successMessage = "err";
                ViewBag.ErrInfo = ex.Message;
                result = false;
            }
            return result;
        }
        #endregion
    }
}