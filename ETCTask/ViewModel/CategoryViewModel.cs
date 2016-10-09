using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ETCTask.Models;

namespace ETCTask.ViewModel
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}