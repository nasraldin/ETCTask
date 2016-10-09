using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using ETCTask.Core;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ETCTask.Models
{
    public class AppDbInitializer : CreateDatabaseIfNotExists<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            InitializeIUsers(context);
            InitializeIData(context);
            base.Seed(context);
        }

        public static void InitializeIUsers(AppDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<UserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<RoleManager>();
            const string username = "admin";
            const string name = "admin@admin.com";
            const string password = "123456";
            const string roleName = "Admin";

            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new Role(roleName);
                roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new User {UserName = username, Email = name};
                userManager.Create(user, password);
            }

            var groupManager = new GroupManager();
            var newGroup = new Group("SuperAdmins", "Full Access");

            groupManager.CreateGroup(newGroup);
            groupManager.SetUserGroups(user.Id, newGroup.Id);
            groupManager.SetGroupRoles(newGroup.Id, role.Name);
        }

        public static void InitializeIData(AppDbContext context)
        {
            var category = new List<Category>
            {
                new Category {CategoryName = "Cars"},
                new Category {CategoryName = "Books"},
                new Category {CategoryName = "Programing"},
                new Category {CategoryName = "Web"}
            };

            var product = new List<Product>
            {
                new Product
                {
                    ProductName = "Toyota",
                    CategoryId = 1,
                    IsFeatured = true,
                    Note = "Lorem Ipsum is simply dummy text",
                    Icon = "CHRY_200_2015.png"
                },
                new Product
                {
                    ProductName = "BMW",
                    CategoryId = 1,
                    IsFeatured = false,
                    Note = "Lorem Ipsum is simply dummy text",
                    Icon = "ddEdit.png"
                },
                new Product
                {
                    ProductName = "Ford",
                    CategoryId = 1,
                    IsFeatured = true,
                    Note = "Lorem Ipsum is simply dummy text",
                    Icon = "FORD_FOCU_2012-1.png"
                },
                new Product
                {
                    ProductName = "Isalmic",
                    CategoryId = 2,
                    IsFeatured = true,
                    Note = "Lorem Ipsum is simply dummy text",
                    Icon = "Preview-icon.png"
                },
                new Product
                {
                    ProductName = "Scientific",
                    CategoryId = 2,
                    Note = "Lorem Ipsum is simply dummy text"
                },
                new Product
                {
                    ProductName = "C#.Net",
                    CategoryId = 3,
                    Note = "Lorem Ipsum is simply dummy text",
                    IsFeatured = true
                },
                new Product
                {
                    ProductName = "ASP.Net",
                    CategoryId = 3,
                    IsFeatured = true,
                    Note = "Lorem Ipsum is simply dummy text"
                },
                new Product
                {
                    ProductName = "HTML 5",
                    CategoryId = 4,
                    IsFeatured = true,
                    Note = "Lorem Ipsum is simply dummy text"
                },
                new Product
                {
                    ProductName = "CSS 3",
                    CategoryId = 4,
                    Note = "Lorem Ipsum is simply dummy text"
                }
            };

            var productDetail = new List<ProductDetail>
            {
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    Image = "2014_nissan_sentra_angularfront.jpg",
                    ProductId = 1
                },
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    Image = "CHRY_200_2015.png",
                    ProductId = 2
                },
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    ProductId = 3
                },
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    ProductId = 4
                },
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    Image = "html.jpg",
                    ProductId = 5
                },
                new ProductDetail
                {
                    Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry.",
                    ProductId = 6
                }
            };

            context.Categories.AddRange(category);
            context.Products.AddRange(product);
            context.ProductDetails.AddRange(productDetail);
            context.SaveChanges();
        }
    }
}