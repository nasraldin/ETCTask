using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ETCTask.Models.Auth
{
    public class Role : IdentityRole<string, UserRole>
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Role(string name)
            : this()
        {
            Name = name;
        }
    }
}