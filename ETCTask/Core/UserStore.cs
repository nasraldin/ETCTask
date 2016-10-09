using System;
using System.Data.Entity;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ETCTask.Core
{
    public class UserStore
        : UserStore<User, Role, string,
                UserLogin, UserRole,
                UserClaim>, IUserStore<User, string>,
            IDisposable
    {
        public UserStore()
            : this(new IdentityDbContext())
        {
            DisposeContext = true;
        }

        public UserStore(DbContext context)
            : base(context)
        {
        }
    }
}