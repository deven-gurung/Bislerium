using Application.Constants;
using Application.DTOs.Email;
using Application.DTOs.Profile;
using Application.Interfaces;
using Domain.Entities.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services;

public class ProfileService(IUserService userService, IEmailService emailService, UserManager<User> userManager, ApplicationDbContext dbContext)
    : IProfileService
{
    public ProfileDetailsDto GetProfileDetails()
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var userRole = dbContext.UserRoles.FirstOrDefault(x => x.UserId == user!.Id);
        
        var role = dbContext.Roles.Find(userRole!.RoleId);

        var result = new ProfileDetailsDto()
        {
            UserId = user!.Id,
            FullName = user.Name,
            Username = user.UserName ?? "",
            EmailAddress = user.Email ?? "",
            RoleId = role!.Id,
            RoleName = role.Name ?? "",
            ImageURL = user.ImageURL ?? "dummy.svg",
            MobileNumber = user.PhoneNumber ?? ""
        };
        
        return result;
    }

    public bool UpdateProfileDetails(ProfileDetailsDto profileDetails)
    {
        var user = dbContext.Users.Find(profileDetails.UserId);

        user!.Name = profileDetails.FullName;
        user.PhoneNumber = profileDetails.MobileNumber;
        user.Email = profileDetails.EmailAddress;

        dbContext.Update(user);

        return true;
    }

    public bool DeleteProfile()
    {
        var userId = userService.UserId;

        var user = dbContext.Users.Find(userId);

        var blogs = dbContext.Blogs.Where(x => x.CreatedBy == user.Id);

        var blogImages = dbContext.BlogImages.Where(x => blogs.Select(z => z.Id).Contains(x.BlogId));

        var comments = dbContext.Comments.Where(x => x.CreatedBy == user.Id);

        var reactions = dbContext.Reactions.Where(x => x.CreatedBy == user.Id);

        dbContext.RemoveRange(reactions);

        dbContext.RemoveRange(comments);

        dbContext.RemoveRange(blogImages);

        dbContext.RemoveRange(blogs);

        dbContext.Users.Remove(user);

        return true;
    }

    public async Task<bool> ChangePassword(ChangePasswordDto changePassword)
    {
        var userId = userService.UserId;

        var user = await dbContext.Users.FindAsync(userId);

        var validPassword = await userManager.CheckPasswordAsync(user!, changePassword.CurrentPassword);

        if (!validPassword) return false;
        
        if (changePassword.ConfirmPassword != changePassword.NewPassword) return false;
        
        var result = await userManager.ChangePasswordAsync(user!, changePassword.CurrentPassword, changePassword.NewPassword);

        return result.Succeeded;
    }

    public async Task<bool> ResetPassword(string emailAddress)
    {
        var user = dbContext.Users.FirstOrDefault(x => x.Email == emailAddress);

        if (user == null)
        {
            return false;
        }

        var passwordHash = new PasswordHasher<User>();

        const string newPassword = Constants.Passwords.BloggerPassword;

        user.PasswordHash = passwordHash.HashPassword(user, newPassword);

        dbContext.Users.Update(user);

        var message =
            $"Hello {user.Name}, <br><br> " +
            $"This is to inform you that your password has been successfully reset as per your request. " +
            $"Your new password is {newPassword}.<br><br>" +
            $"Best regards,<br>" +
            $"Bislerium.";

        var email = new EmailDto()
        {
            Email = user.Email ?? "",
            Message = message,
            Subject = "Reset Password - Bislerium"
        };

        emailService.SendEmail(email);

        return true;
    }
}