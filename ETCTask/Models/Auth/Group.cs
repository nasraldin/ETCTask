using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ETCTask.Models.Auth
{
    public class Group
    {
        public Group()
        {
            Id = Guid.NewGuid().ToString();
            Roles = new List<GroupRole>();
            Users = new List<UserGroup>();
        }

        public Group(string name)
            : this()
        {
            Name = name;
        }

        public Group(string name, string description)
            : this(name)
        {
            Description = description;
        }

        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<GroupRole> Roles { get; set; }
        public virtual ICollection<UserGroup> Users { get; set; }
    }
}