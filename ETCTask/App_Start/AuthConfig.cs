using System.Security.Claims;
using System.Threading.Tasks;
using ETCTask.Core;
using ETCTask.Models;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace ETCTask
{
    public class UserManager : UserManager<User, string>
    {
        public UserManager(IUserStore<User, string> store)
            : base(store)
        {
        }

        public static UserManager Create(IdentityFactoryOptions<UserManager> options, IOwinContext context)
        {
            var manager =
                new UserManager(
                    new UserStore<User, Role, string, UserLogin, UserRole, UserClaim>(context.Get<AppDbContext>()));
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("ASP.NET Identity"));
            return manager;
        }
    }

    public class SignInManager : SignInManager<User, string>
    {
        public SignInManager(UserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(User user)
        {
            return user.GenerateUserIdentityAsync((UserManager) UserManager);
        }

        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
        {
            return new SignInManager(context.GetUserManager<UserManager>(), context.Authentication);
        }
    }

    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(IRoleStore<Role, string> roleStore)
            : base(roleStore)
        {
        }

        public static RoleManager Create(IdentityFactoryOptions<RoleManager> options, IOwinContext context)
        {
            return new RoleManager(new RoleStore(context.Get<AppDbContext>()));
        }
    }
}