using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ETCTask.Models
{
    public class Product
    {
        public Product()
        {
            DateCreated = DateTime.Now;
        }

        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string Note { get; set; }
        public string Icon { get; set; }

        [DefaultValue(false)]
        public bool IsFeatured { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateCreated { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
    }
}