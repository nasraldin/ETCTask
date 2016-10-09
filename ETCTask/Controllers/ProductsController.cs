using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ETCTask.Models;
using PagedList;

namespace ETCTask.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        public JsonResult GetProducts(int id)
        {
            var products = _context.Products.Where(a => a.CategoryId.Equals(id))
                .ToDictionary(p => p.ProductName, p => p.ProductId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,ViewProduct")]
        public ActionResult Index(int? page)
        {
            var products = _context.ProductDetails.Include(p => p.Product.Category).ToList();
            var pageSize = 3;
            var pageNumber = page ?? 1;
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        [Authorize(Roles = "Admin,AddProduct")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(HttpPostedFileBase fileBase,
            [Bind(Include = "ProductDetailId,Description,Image,ProductId")] ProductDetail product)
        {
            if (ModelState.IsValid)
            {
                string[] allowedExtensions = {".jpg", ".png", ".JPG", ".PNG"};
                if (fileBase != null)
                {
                    var extension = Path.GetExtension(fileBase.FileName);
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("CustomError", "files extensions not allowed!");
                    }
                    else
                    {
                        var filename = Path.GetFileName(fileBase.FileName);
                        if (filename != null)
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/image/"), filename);
                            fileBase.SaveAs(path);
                        }
                        product.Image = filename;
                        _context.ProductDetails.Add(product);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.ProductId);
            return View(product);
        }

        public ActionResult ViewDetails(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var product = _context.ProductDetails.Find(id);
            if (product == null)
                return HttpNotFound();
            ViewBag.Product = _context.Products.Find(product.ProductId);
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}