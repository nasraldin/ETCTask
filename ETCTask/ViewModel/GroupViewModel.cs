using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ETCTask.ViewModel
{
    public class GroupViewModel
    {
        public GroupViewModel()
        {
            UsersList = new List<SelectListItem>();
            RolesList = new List<SelectListItem>();
        }

        [Required(AllowEmptyStrings = false)]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public string Description { get; set; }
        public ICollection<SelectListItem> UsersList { get; set; }
        public ICollection<SelectListItem> RolesList { get; set; }
    }
}