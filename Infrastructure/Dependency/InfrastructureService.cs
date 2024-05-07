using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bislerium.Infrastructure.Dependency;

public static class InfrastructureService
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddIdentity<User, Role>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("Infrastructure")));

        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IAdminService, AdminService>();
        services.AddTransient<IBlogService, BlogService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileUploadService, FileUploadService>();
        services.AddTransient<IHomeService, HomeService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<IUserService, UserService>();

        return services;
    }
}