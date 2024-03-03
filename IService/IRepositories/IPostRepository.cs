using Data_Trans_Job.Models;

namespace Data_Trans_Job.IService.IRepositories
{
    public interface IPostRepository
    {
        void DeletePostWithComments(int postId);
         List<Post> SearchPosts(string? searchTerm, DateTime? fromDate);

        Task<List<Post>> GetAllPosts();
        Task<Post> GetPostById(int id);
        Task AddPost(Post post);
        Task UpdatePost(Post post);
        IQueryable<Post> GetPostsByDateAndTitle(DateTime date, string title);
        Task DeletePost(Post post);
        Task AddComment(Comments comments);
    }
}
