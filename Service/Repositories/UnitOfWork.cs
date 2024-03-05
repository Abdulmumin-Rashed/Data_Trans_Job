using Data_Trans_Job.Data;
using Data_Trans_Job.IService.IRepositories;
using Data_Trans_Job.Models;
using Microsoft.AspNetCore.Identity;

namespace Data_Trans_Job.Service.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IAdminRepository Admin { get; private set; }
        public  IRoleRepository Role { get; private set; }
        public IPostRepository Post { get; private set; }
        public UnitOfWork(ApplicationDbContext context, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager , IHttpContextAccessor httpContextAccessor,
             IConfiguration configuration
            , ApplicationDbContext dbContext)
        {
            _context = context;
            Admin = new AdminRepository(userManager, roleManager,httpContextAccessor,configuration,dbContext);
            Role = new RoleRepository(roleManager);
            Post = new PostRepository(context);

        }
 

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
