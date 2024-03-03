using System.ComponentModel.DataAnnotations;

namespace Data_Trans_Job.Areas.Admin.ModelView
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public List<string> SelectedRoles { get; set; }

        public List<string>? AvailableRoles { get; set; }

       
    }
}
