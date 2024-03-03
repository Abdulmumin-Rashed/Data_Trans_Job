using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Data_Trans_Job.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? UrlImage { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comments> Comments { get; set; }
        [NotMapped]
        public List<IdentityRole> Roles { get; set; } = new List<IdentityRole>();
    }
}
