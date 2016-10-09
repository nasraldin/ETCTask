using System.ComponentModel.DataAnnotations;

namespace ETCTask.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Plz Enter Username", AllowEmptyStrings = false)]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Plz Enter Passowrd")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}