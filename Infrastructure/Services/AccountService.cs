using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class AccountService(ApplicationDbContext dbContext, SignInManager<User> signInManager) : IAccountService
{
    public User GetUserByEmail(string email)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Email == email);
        
        return user ?? new User();
    }

    public Guid GetUserRoleId(string email)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Email == email);

        var userRole = dbContext.UserRoles.FirstOrDefault(x => x.UserId == user!.Id);

        return userRole!.RoleId;
    }
    
    public Role GetRoleById(Guid roleId)
    {
        return dbContext.Roles.Find(roleId) ?? new Role();
    }

    public async Task<bool> IsPasswordValid(string email, string password)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Email == email);

        var isPasswordValid = await signInManager.CheckPasswordSignInAsync(user!, password, false);

        return isPasswordValid.Succeeded;
    }
    
    public User GetExistingUser(string emailAddress, string username)
    {
        var user = dbContext.Users.FirstOrDefault(x =>
            x.Email == emailAddress || x.UserName == username);

        return user ?? new User();
    }

    public Role GetRoleByName(string roleName)
    {
        var role = dbContext.Roles.FirstOrDefault(x => x.Name == roleName);

        return role ?? new Role();
    }

    public void InsertUser(User user)
    {
        var role = dbContext.Roles.FirstOrDefault(x => x.Name == "Blogger");

        dbContext.Users.Add(user);

        var userRole = new UserRoles()
        {
            RoleId = role!.Id,
            UserId = user.Id
        };

        dbContext.Add(userRole);
    }
}
