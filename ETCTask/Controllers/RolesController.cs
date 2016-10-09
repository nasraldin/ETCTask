using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ETCTask.Models.Auth;
using ETCTask.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ETCTask.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private RoleManager _roleManager;
        private UserManager _userManager;

        public RolesController()
        {
        }

        public RolesController(UserManager userManager, RoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public UserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
            set { _userManager = value; }
        }

        public RoleManager RoleManager
        {
            get { return _roleManager ?? HttpContext.GetOwinContext().Get<RoleManager>(); }
            private set { _roleManager = value; }
        }

        public ActionResult Index()
        {
            return View(RoleManager.Roles);
        }

        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByIdAsync(id);
            var users = new List<User>();
            foreach (var user in UserManager.Users.ToList())
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                    users.Add(user);

            ViewBag.Users = users;
            ViewBag.UserCount = users.Count();
            return PartialView("_Details", role);
        }

        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new Role(roleViewModel.Name);
                var roleresult = await RoleManager.CreateAsync(role);
                if (!roleresult.Succeeded)
                    ModelState.AddModelError("", roleresult.Errors.First());

                var url = Url.Action("Index", "Roles");
                return Json(new {success = true, url});
            }
            return PartialView("_Create");
        }

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();
            var roleModel = new RoleViewModel {Id = role.Id, Name = role.Name};
            return PartialView("_Edit", roleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Name,Id")] RoleViewModel roleModel)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleManager.FindByIdAsync(roleModel.Id);
                role.Name = roleModel.Name;
                await RoleManager.UpdateAsync(role);
                var url = Url.Action("Index", "Auth");
                return Json(new {success = true, url});
            }
            return PartialView("_Edit");
        }

        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return HttpNotFound();
            return PartialView("_Delete", role);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string deleteUser)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                var role = await RoleManager.FindByIdAsync(id);
                if (role == null)
                    return HttpNotFound();
                IdentityResult result;
                if (deleteUser != null)
                    result = await RoleManager.DeleteAsync(role);
                else
                    result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    ModelState.AddModelError("", result.Errors.First());
                var url = Url.Action("Index", "Auth");
                return Json(new {success = true, url});
            }
            return PartialView("_Delete");
        }
    }
}