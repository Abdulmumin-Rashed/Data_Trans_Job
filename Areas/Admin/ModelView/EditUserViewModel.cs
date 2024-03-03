using Data_Trans_Job.Models;
using Microsoft.AspNetCore.Identity;

namespace Data_Trans_Job.Areas.Admin.ModelView
{
    public class EditUserViewModel
    {
        public AppUser User { get; set; }
        public List<IdentityRole> UserRoles { get; set; }
        public List<IdentityRole> AvailableRoles { get; set; }
        public List<string> SelectedRoles { get; set; }
    }
}
