using Dapper;
using Data_Trans_Job.Data;
using Data_Trans_Job.IService.IRepositories;
using Data_Trans_Job.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Data_Trans_Job.Service.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        public AdminRepository(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration
            , ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<List<AppUser>> GetUsersWithRecentPosts()
        {
            string connectionData = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                DateTime thirtyDaysAgo = DateTime.Now.AddDays(-30);

                // Using Entity Framework Core to query recent posts
                var usersWithRecentPostsEF = await _dbContext.Users
                    .Include(u => u.Posts)
                    .Where(u => u.Posts.Any(p => p.CreatedAt >= thirtyDaysAgo))
                    .ToListAsync();

                // Using Dapper to query recent posts
                using (var connection = new SqlConnection(connectionData))
                {
                    connection.Open();

                    string query = @" SELECT DISTINCT u.*
                                   FROM AspNetUsers u
                                   INNER JOIN Posts p ON u.Id = p.UserId
                                   WHERE p.CreatedAt >= @ThirtyDaysAgo";

                    var usersWithRecentPostsDapper = await connection.QueryAsync<AppUser>(query, new { ThirtyDaysAgo = thirtyDaysAgo });

                    return usersWithRecentPostsDapper.ToList();
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                throw;
            }
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password, List<string> roleNames)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded && roleNames != null)
            {
                var existingRoles = await _roleManager.Roles.ToListAsync();
                var validRoleNames = roleNames.Intersect(existingRoles.Select(r => r.Name));

                foreach (var roleName in validRoleNames)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }

            return result;
        }



        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }
        public async Task<List<AppUser>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                user.Roles = await GetUserRoles(user);
            }

            return users;
        }



        public async Task<List<IdentityRole>> GetUserRoles(AppUser user)
        {
            var roleNames = await _userManager.GetRolesAsync(user);
            var roles = new List<IdentityRole>();

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    roles.Add(role);
                }
            }

            return roles;
        }
        public async Task RemoveUserRoles(AppUser user, List<IdentityRole> roles)
        {
            foreach (var role in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, role.Name);
            }
        }

        public async Task AddUserRoles(AppUser user, List<IdentityRole> roles)
        {
            foreach (var role in roles)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
            }
        }

        public Task<AppUser> GetUserById(string userId)
        {
            return _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityResult> UpdateUser(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public Task DeleteUser(AppUser user)
        {
            return _userManager.DeleteAsync(user);
        }

        public async Task<List<string>> GetRolesAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return (List<string>)roles;

        }
        public async Task<AppUser> GetUserByEmail(string email)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IdentityResult> RemoveUserRoleAsync(AppUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            return result;
        }

        public async Task<IdentityResult> AddUserRoleAsync(AppUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.AddToRolesAsync(user, roles);
            return result;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }
    }
}
