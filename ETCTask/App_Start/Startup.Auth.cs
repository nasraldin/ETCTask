using System;
using ETCTask.Models;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace ETCTask
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(AppDbContext.Create);
            app.CreatePerOwinContext<UserManager>(UserManager.Create);
            app.CreatePerOwinContext<RoleManager>(RoleManager.Create);
            app.CreatePerOwinContext<SignInManager>(SignInManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Auth/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, User, string>(
                        TimeSpan.FromMinutes(10),
                        (manager, user) => user.GenerateUserIdentityAsync(manager),
                        claim => claim.GetUserId())
                }
            });
        }
    }
}