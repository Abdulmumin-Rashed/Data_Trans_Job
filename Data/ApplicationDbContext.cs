using Data_Trans_Job.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data_Trans_Job.Data
{


    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ///Adding roles to the role user identity table

            modelBuilder.Entity<IdentityRole>().HasData
             (
             new IdentityRole() { Name = "Super_Admin", ConcurrencyStamp = "1", NormalizedName = "Super_Admin" },
             new IdentityRole() { Name = "Admin", ConcurrencyStamp = "2", NormalizedName = "Admin" },
             new IdentityRole() { Name = "User", ConcurrencyStamp = "3", NormalizedName = "User" }

             );

            //one to many relationship between user and post

            // Define one-to-many relationship between user and posts
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Define one-to-many relationship between user and comments
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Define one-to-many relationship between post and comments
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.NoAction);





        }

    }

}
