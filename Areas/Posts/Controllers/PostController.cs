using Data_Trans_Job.IService.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Data_Trans_Job.Models;
using Data_Trans_Job.Service.Repositories;
using Microsoft.AspNetCore.Identity;
using Data_Trans_Job.Areas.Posts.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace Data_Trans_Job.Areas.Posts.Controllers
{
    [Area("Posts")]
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            List<Post> posts;


            posts = await _unitOfWork.Post.GetAllPosts();


            return View(posts);
        }

        public async Task<IActionResult> AddComment(int postId, string commentText)
        {
            var post = await _unitOfWork.Post.GetPostById(postId);
            if (post == null)
            {
                return NotFound();
            }

            var currentUser = await _unitOfWork.Admin.GetCurrentUserAsync();
            var comment = new Comments
            {
                PostId = postId,
                UserId = currentUser.Id,
                Text = commentText
            };

            await _unitOfWork.Post.AddComment(comment);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _unitOfWork.Admin.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    var post = new Post
                    {
                        Title = postViewModel.Title,
                        Content = postViewModel.Content,
                        UserId = currentUser.Id
                    };

                    await _unitOfWork.Post.AddPost(post);

                    // SweetAlert success message
                    return Json(new { success = true, message = "Post created successfully!" });
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found. Please login if you have an account or register if you do not.";
                }
            }

            // If model state is invalid, return model state errors
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }

        [Authorize(Roles = "Super_Admin")]

        public async Task<IActionResult> Delete(int id)
        {
           
            _unitOfWork.Post.DeletePostWithComments(id);
            return RedirectToAction(nameof(Index));
        }


        //[HttpPost]
        //public IActionResult Delete(int id)
        //{
        //    _unitOfWork.Post.DeletePostWithComments(id);
        //    return RedirectToAction(nameof(Index)); // Redirect to the appropriate action after deletion
        //}
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _unitOfWork.Post.GetPostById(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post updatedPost)
        {
            if (id != updatedPost.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _unitOfWork.Post.GetPostById(id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    existingPost.Content = updatedPost.Content;
                    existingPost.Title = updatedPost.Title;
                    // Update other properties as needed

                    await _unitOfWork.Post.UpdatePost(existingPost);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "Failed to save changes. The post was modified by another user.");
                }
            }

            return View(updatedPost);
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            List<Post> posts = await _unitOfWork.Post.GetAllPosts();

            // Filter posts based on the search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                posts = posts.Where(p => p.Title.Contains(searchTerm) ||
                                         p.Content.Contains(searchTerm) ||
                                         p.CreatedAt.Date.ToString("yyyy-MM-dd HH:mm:ss").Contains(searchTerm)).ToList();
            }

            return PartialView("_PostsTable", posts);
        }
    }
}
