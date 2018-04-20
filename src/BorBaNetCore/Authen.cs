using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BorBaNetCore
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Authen
    {
        private readonly RequestDelegate _next;

        public Authen(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthenExtensions
    {
        public static IApplicationBuilder UseAuthen(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Authen>();
        }
    }
}
