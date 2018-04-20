
using BorBaNetCore.Models;
using BorBaNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BorBaNetCore.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Threading;

namespace BorBaNetCore.MiddleWare
{
    public class Authorization
    {
        private static readonly PathString _API_PATH = new PathString("/api");

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public Authorization(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<Authorization>();
        }

        //		public async Task Invoke(HttpContext context, IUserManager userManager)
        //		{
        //			// We let login and API urls to go through, as they have special handling.
        //			if (context.Request.Path.Equals(Constants.LOGIN_PAGE, StringComparison.OrdinalIgnoreCase)
        //				|| context.Request.Path.StartsWithSegments(_API_PATH))
        //			{
        //				try
        //				{
        //					await _next.Invoke(context);
        //				}
        //				catch (Exception ex)
        //				{
        //					_logger.LogCritical(ex.ToString());
        //#if DEBUG
        //					throw ex;
        //#endif
        //				}
        //			}
        //			else
        //			{
        //				_logger.LogInformation("Handling request: " + context.Request.Path);
        //				if (IsAuthorized(context, userManager).Result)
        //					await _next.Invoke(context);
        //				_logger.LogInformation("Finished handling request.");
        //			}
        //		}

        public async Task Invoke(HttpContext context, IUserManager userManager)
        {
            string cultureName = Thread.CurrentThread.CurrentCulture.Name;
            string header = context.Request.Headers["Accept-Language"];          
            if (!string.IsNullOrEmpty(header))
                cultureName = header.Split(',')[0];

            // Attempt to read the culture cookie from Request
            string cultureCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            if (!string.IsNullOrEmpty(cultureCookie))
                cultureName = cultureCookie.Split('|')[0].Replace("c=","");


            // Modify current thread's cultures            
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
          
            await _next.Invoke(context);
        }
        protected async virtual Task<bool> IsAuthorized(HttpContext context, IUserManager userManager)
        {
            CurrentUser user = await userManager.GetCurrentUser(context);

            string pwd = GetHashedPassword("admin");
            if (user == null)
            {
                if (AcceptHtml(context.Request))
                {
                    string query = string.Empty;
                    if (context.Request.Path.IsNotEmpty() && context.Request.Path != "/")
                    {
                        query += "?returnUrl=" + context.Request.Path;
                    }
                    if (context.Request.QueryString.IsNotEmpty())
                    {
                        query += context.Request.QueryString.Value;
                    }
                    context.Response.Redirect(Constants.LOGIN_PAGE + query);
                }
                else
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                }
                return false;
            }
            return true;
        }

        private bool AcceptHtml(HttpRequest request)
        {
            const string header_ACCEPT = "Accept", accept_HTML = "text/html";
            if (request.Headers.ContainsKey(header_ACCEPT))
            {
                return request.Headers[header_ACCEPT].Any(s => s.IndexOf(accept_HTML, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return false;
        }

        public string GetHashedPassword(string password)
        {
            string Salt = Guid.NewGuid().ToString();
            return (password + Salt).MD5Hash();
        }
    }
}
