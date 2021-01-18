using CommonBasicStandardLibraries.ContainerClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Runtime.InteropServices;
using System.Text;
using cc = CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
namespace SimpleJwtServerAuthenticationLibrary
{
    public static class Extensions
    {
        //private static ContainerMain _main;
        //public static ContainerMain GetMainContainer()
        //{
        //    return _main;
        //}



        public static void AddJwtAuthentication<K>(this IServiceCollection services, Action<ContainerMain> options = null, bool AddUtilityService = true)
            where K: class, IServerJwtKey
        {
            //looks like i have to use my custom one for this.

            ContainerMain main = new ();
            cc.cons = main;
            main.RegisterSingleton<IServerJwtKey, K>();
            //for this, has to use my custom one.  can register for off the shelf too though.



            var key = cc.cons.Resolve<K>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key.GetKey())),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddSingleton<IServerJwtKey, K>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            if (AddUtilityService)
            {
                services.AddScoped<ISecurityUtilityService, SecurityUtilityService>();
            }
            if (options != null)
            {
                options.Invoke(main);
            }
        }

        public static void AddJwtAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

    }
}