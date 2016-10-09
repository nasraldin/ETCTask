using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ETCTask.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}