using System.ComponentModel.DataAnnotations;

namespace MyMVC.Models.ViewModel
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }
        [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w+$", ErrorMessage = "Login ID should be a valid email address.")]
        [Required(ErrorMessage = "*")]
        [Display(Name = "Login ID")]
        public string LoginName { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "*")]
        [StringLength(50, ErrorMessage = "Maximum length is 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "Created By")]
        public int CreatedBy { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string AccountImage { get; set; }

        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class UsersModel
    {
        public List<UserModel> Users { get; set; }
    }

    public class UserLoginModel
    {
        [Key]
        public int SYSUserId { get; set; }
        [Required(ErrorMessage = "*")]
        [Display(Name = "LoginID")]
        public string LoginName { get; set; }
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
