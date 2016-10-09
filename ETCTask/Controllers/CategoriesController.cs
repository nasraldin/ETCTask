using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ETCTask.Models;
using ETCTask.ViewModel;

namespace ETCTask.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        [Authorize(Roles = "Admin,ViewCategory")]
        public async Task<ActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return HttpNotFound();
            return PartialView(category);
        }

        [Authorize(Roles = "Admin,AddCategory")]
        public ActionResult AddCategory()
        {
            ViewBag.Title = "Add New Category";
            return View("CatForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(
            [Bind(Include = "CategoryId,CategoryName")] CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                var cat = new Category {CategoryName = category.CategoryName};
                _context.Categories.Add(cat);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View("CatForm", category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return HttpNotFound();
            ViewBag.Title = "Edit Category";
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update([Bind(Include = "CategoryId,CategoryName")] Category category)
        {
            if (!ModelState.IsValid) return View(category);
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,DeleteCategory")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return HttpNotFound();
            return PartialView(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            var url = Url.Action("Index", "Categories");
            return Json(new {success = true, url});
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}