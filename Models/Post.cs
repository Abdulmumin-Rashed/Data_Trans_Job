using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Data_Trans_Job.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(500, ErrorMessage = "Content must be less than 500 characters")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.AddDays(-30);

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public ICollection<Comments> Comments { get; set; }
    }

}
