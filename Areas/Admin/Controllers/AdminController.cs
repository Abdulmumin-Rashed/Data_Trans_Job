using Data_Trans_Job.Areas.Admin.ModelView;
using Data_Trans_Job.IService.IRepositories;
using Data_Trans_Job.Models;
using Data_Trans_Job.Service.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;

namespace Data_Trans_Job.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Super_Admin")]
    public class AdminController : Controller
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        public AdminController(IUnitOfWork unitOfWork , IHubContext<NotificationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _unitOfWork.Admin.GetAllUsersWithRolesAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var availableRoles = _unitOfWork.Admin.GetAllRolesAsync().GetAwaiter().GetResult();

            var viewModel = new CreateUserViewModel
            {
                AvailableRoles = availableRoles.Select(r => r.Name).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = viewModel.Email,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Address = viewModel.Address
                };

                if (viewModel.Password is not null)
                {
                    var result = await _unitOfWork.Admin.CreateUserAsync(user, viewModel.Password, viewModel.SelectedRoles);
                    if (result.Succeeded)
                    {
                        return Json(new { success = true, message = "Post created successfully!" });

                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            viewModel.AvailableRoles = _unitOfWork.Admin.GetAllRolesAsync().GetAwaiter().GetResult()
                .Select(r => r.Name).ToList();
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
       
        }




        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            // Retrieve the user with the specified ID from the database
            var user = await _unitOfWork.Admin.GetUserById(userId);

            if (user == null)
            {
                // User not found, return a not found error
                return NotFound();
            }

            // Retrieve all available roles from the database
            var availableRoles = await _unitOfWork.Role.GetAllRolesAsync();
          var roles = await _unitOfWork.Admin.GetUserRoles(user);
            // Map the user and available roles to the view model
            var viewModel = new CreateUserViewModel
            {
                Email = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                AvailableRoles = availableRoles.Select(r => r.Name).ToList(),
                SelectedRoles = roles.Select(r => r.Name).ToList()

            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreateUserViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Model validation failed, return the view with validation errors
                viewModel.AvailableRoles = (List<string>)await _unitOfWork.Role.GetAllRolesAsync();
                return View(viewModel);
            }

            // Retrieve the user with the specified ID from the database
            var user = await _unitOfWork.Admin.GetUserByEmail(viewModel.Email);

            if (user == null)
            {
                // User not found, return a not found error
                return NotFound();
            }

            // Update the user properties with the new values
            user.UserName = viewModel.Email;
            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.Address = viewModel.Address;

            // Get the current roles of the user
            var userRoles = await _unitOfWork.Admin.GetRolesAsync(user);

            // Remove old roles that are not selected anymore
            var rolesToRemove = userRoles.Except(viewModel.SelectedRoles);
            await _unitOfWork.Admin.RemoveUserRoleAsync(user, rolesToRemove);

            // Add new roles that are selected
            var rolesToAdd = viewModel.SelectedRoles.Except(userRoles);
            await _unitOfWork.Admin.AddUserRoleAsync(user, rolesToAdd);

            // Update the user in the database
            var result = await _unitOfWork.Admin.UpdateUser(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index"); // Redirect to a success page
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // User update failed, return the view with error messages
                viewModel.AvailableRoles = (List<string>?)await _unitOfWork.Admin.GetAllRolesAsync();
                return View(viewModel);
            }
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _unitOfWork.Admin.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            await _unitOfWork.Admin.DeleteUser(user);
            TempData["success"] = "Category Deleted Successfully";
            return Json(new { success = true });
        }

        public async Task<IActionResult> Details(string userId)
        {
            AppUser user = await _unitOfWork.Admin.GetUserById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
    }
}
