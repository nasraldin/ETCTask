using System;

namespace ETCTask.Models
{
    public class ProductDetail
    {
        public ProductDetail()
        {
            DateCreated = DateTime.Now;
        }

        public int ProductDetailId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}