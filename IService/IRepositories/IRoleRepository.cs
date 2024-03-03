using Microsoft.AspNetCore.Identity;

namespace Data_Trans_Job.IService.IRepositories
{
    public interface IRoleRepository
    {
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(string roleName);
        Task<IdentityRole?> GetRoleByIdAsync(string roleId);
        Task<IdentityRole?> GetRoleByNameAsync(string roleName);
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    }
}
