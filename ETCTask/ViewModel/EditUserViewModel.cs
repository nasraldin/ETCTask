using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ETCTask.ViewModel
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            RolesList = new List<SelectListItem>();
            GroupsList = new List<SelectListItem>();
        }

        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<SelectListItem> RolesList { get; set; }
        public ICollection<SelectListItem> GroupsList { get; set; }
    }
}