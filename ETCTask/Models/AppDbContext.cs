using System.Data.Entity;
using ETCTask.Models.Auth;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ETCTask.Models
{
    public class AppDbContext : IdentityDbContext<User, Role, string, UserLogin, UserRole, UserClaim>
    {
        public AppDbContext()
            : base("AppConnection")
        {
            Database.SetInitializer(new AppDbInitializer());
        }

        public virtual IDbSet<Group> Groups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Users)
                .WithRequired().HasForeignKey(ag => ag.GroupId);
            modelBuilder.Entity<UserGroup>()
                .HasKey(r =>
                    new
                    {
                        r.UserId,
                        r.GroupId
                    }).ToTable("UserGroups");

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Roles)
                .WithRequired().HasForeignKey(ap => ap.GroupId);
            modelBuilder.Entity<GroupRole>().HasKey(gr =>
                new
                {
                    gr.RoleId,
                    gr.GroupId
                }).ToTable("GroupRoles");

            modelBuilder.Entity<Group>().ToTable("Groups");
            modelBuilder.Entity<User>().ToTable("Users").
                Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims").
                Property(p => p.Id).HasColumnName("UserClaimId");
            modelBuilder.Entity<Role>().ToTable("Roles").
                Property(p => p.Id).HasColumnName("RoleId");
        }
    }
}