using BorBaNetCore.MiddleWare;
using BorBaNetCore.DataModel;
using BorBaNetCore.Services;
using BorBaNetCore.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace BorBaNetCore.DI
{
    public static class Extensions
    {
        public static void AddBorba(this IServiceCollection services, IConfigurationRoot configuration)
        {
            
            services.AddScoped<DbContext>( _ => _.GetService<BorBaContext>());
            services.AddScoped(
                _ =>
                {
                    ILogger logger = _.GetService<ILoggerFactory>().CreateLogger<UnitOfWork>();
                    return new UnitOfWork(_.GetService<DbContext>(), s => logger.LogInformation(s));
                }
            );
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IMessageManager, MessageManager>();

            #region Transient Instances

           // services.AddTransient(_ => _.GetService<DbContext>() as IDbProcedures);

            #endregion
        }


        /// <summary>
		/// Method to configure application to use MSJ authorization
		/// </summary>
		/// <param name="app"></param>
		public static void UseAuthorization(this IApplicationBuilder app)
        {
            app.UseMiddleware<Authorization>();
        }

       

    }
}
