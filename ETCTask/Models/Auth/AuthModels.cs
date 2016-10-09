using Microsoft.AspNet.Identity.EntityFramework;

namespace ETCTask.Models.Auth
{
    public class UserLogin : IdentityUserLogin<string>
    {
    }

    public class UserClaim : IdentityUserClaim<string>
    {
    }

    public class UserRole : IdentityUserRole<string>
    {
    }
}