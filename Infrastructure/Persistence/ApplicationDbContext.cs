using System.Reflection;
using Application.Constants;
using Domain.Entities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<User, Role, Guid, UserClaims, UserRoles, UserLogin, RoleClaims, UserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Blog> Blogs { get; set; }
    
    public DbSet<BlogImage> BlogImages { get; set; }

    public DbSet<BlogLog> BlogLogs { get; set; }
    
    public DbSet<Comment> Comments { get; set; }

    public DbSet<CommentLog> CommentLogs { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Reaction> Reactions { get; set; }
    
    public DbSet<Role> Roles { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        base.OnModelCreating(builder);

        #region Entities
        builder.Entity<User>().ToTable("Users");
        builder.Entity<Role>().ToTable("Roles");
        builder.Entity<UserToken>().ToTable("Tokens");
        builder.Entity<UserRoles>().ToTable("UserRoles");
        builder.Entity<RoleClaims>().ToTable("RoleClaims");
        builder.Entity<UserClaims>().ToTable("UserClaims");
        builder.Entity<UserLogin>().ToTable("LoginAttempts");
        #endregion

        #region Configurations
		builder.Entity<Blog>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});

		builder.Entity<BlogImage>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});

		builder.Entity<BlogLog>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});

		builder.Entity<Comment>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});

		builder.Entity<CommentLog>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});

		builder.Entity<Notification>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);

			entity.HasOne<User>(n => n.Receiver)
				.WithMany()
				.HasForeignKey(n => n.ReceiverId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);

			entity.HasOne<User>(n => n.Sender)
				.WithMany()
				.HasForeignKey(n => n.SenderId)
				.IsRequired()
				.OnDelete(DeleteBehavior.Restrict);
		});

		builder.Entity<Reaction>(entity =>
		{
			entity.HasOne<User>(b => b.CreatedUser)
				.WithMany()
				.HasForeignKey(b => b.CreatedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.UpdatedUser)
				.WithMany()
				.HasForeignKey(b => b.LastModifiedBy)
				.IsRequired(false);

			entity.HasOne<User>(b => b.DeletedUser)
				.WithMany()
				.HasForeignKey(b => b.DeletedBy)
				.IsRequired(false);
		});
        #endregion
		
		#region Identity Seed
		var superAdminRoleId = Guid.NewGuid();
		var superAdminUserId = Guid.NewGuid();
		var bloggerRoleId = Guid.NewGuid();
        
		var superAdminRole = new Role
		{
			Id = superAdminRoleId, 
			Name = "Super Admin", 
			NormalizedName = "SUPER ADMIN", 
			ConcurrencyStamp = Guid.NewGuid().ToString()
		};
		
		var bloggerRole = new Role
		{
			Id = bloggerRoleId, 
			Name = "Blogger", 
			NormalizedName = "BLOGGER", 
			ConcurrencyStamp = Guid.NewGuid().ToString()
		};

		var superAdminUser = new User()
		{
			Id = superAdminUserId,
			Name = "Super Admin",
			Email = "superadmin@user.com",
			UserName = "superadmin@user.com",
			NormalizedEmail = "SUPERADMIN@USER.COM",
			NormalizedUserName = "SUPERADMIN@USER.COM",
			PhoneNumber = "9800000000",
			EmailConfirmed = true,
			PhoneNumberConfirmed = true,
			SecurityStamp = Guid.NewGuid().ToString("D"),
			ImageURL = "",
		};
        
		var superAdminUserRole = new UserRoles()
		{
			RoleId = superAdminRoleId,
			UserId = superAdminUserId
		};
        
		var passwordHash = new PasswordHasher<User>();
		
		const string password = Constants.Passwords.AdminPassword;
        
		superAdminUser.PasswordHash = passwordHash.HashPassword(superAdminUser, password);
        
		builder.Entity<User>().HasData(superAdminUser);
		builder.Entity<Role>().HasData(superAdminRole);
		builder.Entity<UserRoles>().HasData(superAdminUserRole);
        
		builder.Entity<Role>().HasData(bloggerRole);
		#endregion
	}
}