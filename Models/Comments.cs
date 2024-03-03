using System.ComponentModel.DataAnnotations;

namespace Data_Trans_Job.Models
{

    public class Comments
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Text is required")]
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        // Foreign key for Post
        public int PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }

}
