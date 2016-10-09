using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ETCTask.Models;

namespace ETCTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        public ActionResult Index()
        {
            var product =
                _context.Products.Where(p => p.IsFeatured)
                    .Include(c => c.Category)
                    .OrderBy(p => p.ProductId)
                    .Take(3)
                    .ToList();
            ViewBag.category = _context.Categories.Include(c => c.Products).ToList();

            return View(product);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = _context.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return PartialView("_detailForm", product);
        }

        public ActionResult DetailsV(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = _context.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            return View("detailForm", product);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoadMoreProduct(int size)
        {
            var model = _context.Products.Where(p => p.IsFeatured).OrderBy(p => p.ProductId).Skip(size).Take(3);
            var modelCount = _context.Products.Count(p => p.IsFeatured);
            if (model.Any())
            {
                var modelString = RenderProductView("__MoreProduct", model);
                return Json(new {ModelString = modelString, ModelCount = modelCount});
            }
            return Json(model);
        }

        public string RenderProductView(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}