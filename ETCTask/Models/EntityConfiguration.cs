using System.Data.Entity.ModelConfiguration;

namespace ETCTask.Models
{
    public class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
        {
            ToTable("Categories");
            Property(c => c.CategoryName).IsRequired().HasMaxLength(60);
        }
    }

    public class ProductDetailConfiguration : EntityTypeConfiguration<ProductDetail>
    {
        public ProductDetailConfiguration()
        {
            ToTable("ProductDetails");
            Property(g => g.Description).IsRequired().HasMaxLength(200);
            Property(g => g.ProductId).IsRequired();
        }
    }
}