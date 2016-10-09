using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ETCTask.Core;
using ETCTask.Models.Auth;
using ETCTask.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace ETCTask.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        private GroupManager _groupManager;
        private RoleManager _roleManager;
        private SignInManager _signInManager;
        private UserManager _userManager;

        public AuthController()
        {
        }

        public AuthController(UserManager userManager, SignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public SignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<SignInManager>(); }
            private set { _signInManager = value; }
        }

        public UserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
            private set { _userManager = value; }
        }

        public GroupManager GroupManager
        {
            get { return _groupManager ?? new GroupManager(); }
            private set { _groupManager = value; }
        }

        public RoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext()
                           .Get<RoleManager>();
            }
            private set { _roleManager = value; }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

        public ActionResult AddUser()
        {
            ViewBag.GroupsList = new SelectList(GroupManager.Groups, "Id", "Name");
            return PartialView("_AddUser");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(AddUserViewModel userViewModel, params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email
                };
                var adminresult = await UserManager
                    .CreateAsync(user, userViewModel.Password);

                if (adminresult.Succeeded)
                    if (selectedGroups != null)
                    {
                        selectedGroups = selectedGroups ?? new string[] {};
                        await GroupManager
                            .SetUserGroupsAsync(user.Id, selectedGroups);
                    }
                var url = Url.Action("Index", "Auth");
                return Json(new {success = true, url});
            }

            ViewBag.GroupsList = new SelectList(GroupManager.Groups, "Id", "Name");
            ViewBag.Groups = new SelectList(await RoleManager.Roles.ToListAsync(), "Id", "Name");
            return PartialView("_AddUser");
        }

        [AllowAnonymous]
        public ActionResult Login(string url)
        {
            ViewBag.ReturnUrl = url;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string url)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result == SignInStatus.Success)
                return RedirectTo(url);
            ModelState.AddModelError("", "Invalid login.");
            return View(model);
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);
            var userGroups = await GroupManager.GetUserGroupsAsync(id);
            ViewBag.GroupNames = userGroups.Select(u => u.Name).ToList();
            return PartialView("_Details", user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return HttpNotFound();

            var allGroups = GroupManager.Groups;
            var userGroups = await GroupManager.GetUserGroupsAsync(id);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email
            };

            foreach (var group in allGroups)
            {
                var listItem = new SelectListItem
                {
                    Text = group.Name,
                    Value = group.Id,
                    Selected = userGroups.Any(g => g.Id == group.Id)
                };
                model.GroupsList.Add(listItem);
            }
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            [Bind(Include = "Email,Id")] EditUserViewModel editUser, params string[] selectedGroups)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                    return HttpNotFound();

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                await UserManager.UpdateAsync(user);
                selectedGroups = selectedGroups ?? new string[] {};
                await GroupManager.SetUserGroupsAsync(user.Id, selectedGroups);
                var url = Url.Action("Index", "Auth");
                return Json(new {success = true, url});
            }
            ModelState.AddModelError("", "failed.");
            return PartialView("_Edit");
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
                return HttpNotFound();
            return PartialView("_Delete", user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                    return HttpNotFound();

                await GroupManager.ClearUserGroupsAsync(id);
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                    ModelState.AddModelError("", result.Errors.First());
                var url = Url.Action("Index", "Auth");
                return Json(new {success = true, url});
            }
            return PartialView("_Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectTo(string url)
        {
            if (Url.IsLocalUrl(url))
                return Redirect(url);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}