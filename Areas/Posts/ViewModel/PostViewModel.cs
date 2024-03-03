using System.ComponentModel.DataAnnotations;

namespace Data_Trans_Job.Areas.Posts.ViewModel
{
    public class PostViewModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.AddDays(-30);
        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        public string Content { get; set; }
    }
}
