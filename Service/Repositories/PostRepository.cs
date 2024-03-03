using Data_Trans_Job.Data;
using Data_Trans_Job.IService.IRepositories;
using Data_Trans_Job.Models;
using Microsoft.EntityFrameworkCore;

namespace Data_Trans_Job.Service.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PostRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Post>> GetAllPosts()
        {
            return await _dbContext.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .ToListAsync();
        }

        public async Task<Post> GetPostById(int id)
        {
            return await _dbContext.Posts.FindAsync(id);
        }

        public async Task AddPost(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdatePost(Post post)
        {
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeletePost(Post post)
        {
            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddComment(Comments comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();
        }
        public IQueryable<Post> GetPostsByDateAndTitle(DateTime date, string title)
        {
            var postsByDateAndTitle = _dbContext.Posts
                .Where(p => p.CreatedAt.Date == date.Date && p.Title.Contains(title))
                .Include(p => p.User).Include(p => p.Comments);




            return postsByDateAndTitle;
        }
        public List<Post> SearchPosts(string? searchTerm, DateTime? fromDate)
        {
            IQueryable<Post> query = _dbContext.Posts.Include(p => p.User).Include(p => p.Comments);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt.Date >= fromDate.Value.Date);
            }

            List<Post> searchResults = query.ToList();

            return searchResults;
        }

        public void DeletePostWithComments(int postId)
        {
            var post = _dbContext.Posts
             .Include(p => p.Comments)
             .FirstOrDefault(p => p.Id == postId);

            if (post != null)
            {
                foreach (var comment in post.Comments.ToList())
                {
                    _dbContext.Comments.Remove(comment);
                }

                _dbContext.Posts.Remove(post);
                _dbContext.SaveChanges();
            }
        }
    
    }
}
