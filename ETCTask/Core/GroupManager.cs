using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ETCTask.Models;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ETCTask.Core
{
    public class GroupManager
    {
        private readonly AppDbContext _db;
        private readonly GroupStore _groupStore;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;

        public GroupManager()
        {
            _db = HttpContext.Current
                .GetOwinContext().Get<AppDbContext>();
            _userManager = HttpContext.Current
                .GetOwinContext().GetUserManager<UserManager>();
            _roleManager = HttpContext.Current
                .GetOwinContext().Get<RoleManager>();
            _groupStore = new GroupStore(_db);
        }


        public IQueryable<Group> Groups
        {
            get { return _groupStore.Groups; }
        }


        public async Task<IdentityResult> CreateGroupAsync(Group group)
        {
            await _groupStore.CreateAsync(group);
            return IdentityResult.Success;
        }


        public IdentityResult CreateGroup(Group group)
        {
            _groupStore.Create(group);
            return IdentityResult.Success;
        }


        public IdentityResult SetGroupRoles(string groupId, params string[] roleNames)
        {
            var thisGroup = FindById(groupId);
            thisGroup.Roles.Clear();
            _db.SaveChanges();

            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name));
            foreach (var role in newRoles)
                thisGroup.Roles.Add(new GroupRole {GroupId = groupId, RoleId = role.Id});
            _db.SaveChanges();

            foreach (var groupUser in thisGroup.Users)
                RefreshUserGroupRoles(groupUser.UserId);
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> SetGroupRolesAsync(string groupId, params string[] roleNames)
        {
            var thisGroup = await FindByIdAsync(groupId);
            thisGroup.Roles.Clear();
            await _db.SaveChangesAsync();

            var newRoles = _roleManager.Roles.Where(r => roleNames.Any(n => n == r.Name));
            foreach (var role in newRoles)
                thisGroup.Roles.Add(new GroupRole {GroupId = groupId, RoleId = role.Id});
            await _db.SaveChangesAsync();

            foreach (var groupUser in thisGroup.Users)
                await RefreshUserGroupRolesAsync(groupUser.UserId);
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> SetUserGroupsAsync(string userId, params string[] groupIds)
        {
            var currentGroups = await GetUserGroupsAsync(userId);
            foreach (var group in currentGroups)
                @group.Users
                    .Remove(@group.Users.FirstOrDefault(gr => gr.UserId == userId));
            await _db.SaveChangesAsync();

            foreach (var groupId in groupIds)
            {
                var newGroup = await FindByIdAsync(groupId);
                newGroup.Users.Add(new UserGroup {UserId = userId, GroupId = groupId});
            }
            await _db.SaveChangesAsync();

            await RefreshUserGroupRolesAsync(userId);
            return IdentityResult.Success;
        }


        public IdentityResult SetUserGroups(string userId, params string[] groupIds)
        {
            var currentGroups = GetUserGroups(userId);
            foreach (var group in currentGroups)
                group.Users
                    .Remove(group.Users.FirstOrDefault(gr => gr.UserId == userId));
            _db.SaveChanges();

            foreach (var groupId in groupIds)
            {
                var newGroup = FindById(groupId);
                newGroup.Users.Add(new UserGroup {UserId = userId, GroupId = groupId});
            }
            _db.SaveChanges();

            RefreshUserGroupRoles(userId);
            return IdentityResult.Success;
        }


        public IdentityResult RefreshUserGroupRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            if (user == null)
                throw new ArgumentNullException("User");

            var oldUserRoles = _userManager.GetRoles(userId);
            if (oldUserRoles.Count > 0)
                _userManager.RemoveFromRoles(userId, oldUserRoles.ToArray());

            var newGroupRoles = GetUserGroupRoles(userId);
            var allRoles = _roleManager.Roles.ToList();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.RoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();
            _userManager.AddToRoles(userId, roleNames);

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> RefreshUserGroupRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentNullException("User");

            var oldUserRoles = await _userManager.GetRolesAsync(userId);
            if (oldUserRoles.Count > 0)
                await _userManager.RemoveFromRolesAsync(userId, oldUserRoles.ToArray());

            var newGroupRoles = await GetUserGroupRolesAsync(userId);
            var allRoles = await _roleManager.Roles.ToListAsync();
            var addTheseRoles = allRoles.Where(r => newGroupRoles.Any(gr => gr.RoleId == r.Id));
            var roleNames = addTheseRoles.Select(n => n.Name).ToArray();
            await _userManager.AddToRolesAsync(userId, roleNames);

            return IdentityResult.Success;
        }


        public async Task<IdentityResult> DeleteGroupAsync(string groupId)
        {
            var group = await FindByIdAsync(groupId);
            if (group == null)
                throw new ArgumentNullException("User");

            var currentGroupMembers = (await GetGroupUsersAsync(groupId)).ToList();
            group.Roles.Clear();
            group.Users.Clear();
            _db.Groups.Remove(group);
            await _db.SaveChangesAsync();
            foreach (var user in currentGroupMembers)
                await RefreshUserGroupRolesAsync(user.Id);
            return IdentityResult.Success;
        }


        public IdentityResult DeleteGroup(string groupId)
        {
            var group = FindById(groupId);
            if (group == null)
                throw new ArgumentNullException("User");

            var currentGroupMembers = GetGroupUsers(groupId).ToList();
            group.Roles.Clear();
            group.Users.Clear();
            _db.Groups.Remove(group);
            _db.SaveChanges();
            foreach (var user in currentGroupMembers)
                RefreshUserGroupRoles(user.Id);
            return IdentityResult.Success;
        }


        public async Task<IdentityResult> UpdateGroupAsync(Group group)
        {
            await _groupStore.UpdateAsync(group);
            foreach (var groupUser in group.Users)
                await RefreshUserGroupRolesAsync(groupUser.UserId);
            return IdentityResult.Success;
        }


        public IdentityResult UpdateGroup(Group group)
        {
            _groupStore.Update(group);
            foreach (var groupUser in group.Users)
                RefreshUserGroupRoles(groupUser.UserId);
            return IdentityResult.Success;
        }


        public IdentityResult ClearUserGroups(string userId)
        {
            return SetUserGroups(userId);
        }


        public async Task<IdentityResult> ClearUserGroupsAsync(string userId)
        {
            return await SetUserGroupsAsync(userId);
        }


        public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
        {
            var result = new List<Group>();
            var userGroups = (from g in Groups
                where g.Users.Any(u => u.UserId == userId)
                select g).ToListAsync();
            return await userGroups;
        }


        public IEnumerable<Group> GetUserGroups(string userId)
        {
            var result = new List<Group>();
            var userGroups = (from g in Groups
                where g.Users.Any(u => u.UserId == userId)
                select g).ToList();
            return userGroups;
        }


        public async Task<IEnumerable<Role>> GetGroupRolesAsync(string groupId)
        {
            var grp = await _db.Groups.FirstOrDefaultAsync(g => g.Id == groupId);
            var roles = await _roleManager.Roles.ToListAsync();
            var groupRoles = (from r in roles
                where grp.Roles.Any(ap => ap.RoleId == r.Id)
                select r).ToList();
            return groupRoles;
        }


        public IEnumerable<Role> GetGroupRoles(string groupId)
        {
            var grp = _db.Groups.FirstOrDefault(g => g.Id == groupId);
            var roles = _roleManager.Roles.ToList();
            var groupRoles = from r in roles
                where grp.Roles.Any(ap => ap.RoleId == r.Id)
                select r;
            return groupRoles;
        }


        public IEnumerable<User> GetGroupUsers(string groupId)
        {
            var group = FindById(groupId);
            var users = new List<User>();
            foreach (var groupUser in group.Users)
            {
                var user = _db.Users.Find(groupUser.UserId);
                users.Add(user);
            }
            return users;
        }


        public async Task<IEnumerable<User>> GetGroupUsersAsync(string groupId)
        {
            var group = await FindByIdAsync(groupId);
            var users = new List<User>();
            foreach (var groupUser in group.Users)
            {
                var user = await _db.Users
                    .FirstOrDefaultAsync(u => u.Id == groupUser.UserId);
                users.Add(user);
            }
            return users;
        }


        public IEnumerable<GroupRole> GetUserGroupRoles(string userId)
        {
            var userGroups = GetUserGroups(userId);
            var userGroupRoles = new List<GroupRole>();
            foreach (var group in userGroups)
                userGroupRoles.AddRange(group.Roles.ToArray());
            return userGroupRoles;
        }


        public async Task<IEnumerable<GroupRole>> GetUserGroupRolesAsync(string userId)
        {
            var userGroups = await GetUserGroupsAsync(userId);
            var userGroupRoles = new List<GroupRole>();
            foreach (var group in userGroups)
                userGroupRoles.AddRange(@group.Roles.ToArray());
            return userGroupRoles;
        }


        public async Task<Group> FindByIdAsync(string id)
        {
            return await _groupStore.FindByIdAsync(id);
        }


        public Group FindById(string id)
        {
            return _groupStore.FindById(id);
        }
    }
}