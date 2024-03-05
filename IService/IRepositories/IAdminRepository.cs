using Data_Trans_Job.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Data_Trans_Job.IService.IRepositories
{
    public interface IAdminRepository
    {
        Task<AppUser> GetCurrentUserAsync();
        Task<IdentityResult> RemoveUserRoleAsync(AppUser user, IEnumerable<string> roles);
        Task<IdentityResult> AddUserRoleAsync(AppUser user, IEnumerable<string> roles);
        Task<IdentityResult> CreateUserAsync(AppUser user, string password, List<string> roleNames);
        Task<AppUser> GetUserByEmail(string email);
        Task<List<AppUser>> GetUsersWithRecentPosts();
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<List<AppUser>> GetAllUsersWithRolesAsync();
        Task<List<string>> GetRolesAsync(AppUser user);

        Task<List<IdentityRole>> GetUserRoles(AppUser user);
        Task<AppUser> GetUserById(string userId);
        Task<IdentityResult> UpdateUser(AppUser user);
        Task DeleteUser(AppUser user);
        Task RemoveUserRoles(AppUser user, List<IdentityRole> roles);
        Task AddUserRoles(AppUser user, List<IdentityRole> roles);
    }
}
