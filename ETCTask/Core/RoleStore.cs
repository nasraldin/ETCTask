using System;
using System.Data.Entity;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ETCTask.Core
{
    public class RoleStore
        : RoleStore<Role, string, UserRole>,
            IQueryableRoleStore<Role, string>,
            IRoleStore<Role, string>, IDisposable
    {
        public RoleStore()
            : base(new IdentityDbContext())
        {
            DisposeContext = true;
        }

        public RoleStore(DbContext context)
            : base(context)
        {
        }
    }
}