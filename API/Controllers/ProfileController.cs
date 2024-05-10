using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Records;
using Domain.Entities;
using Domain.Entities.Identity;
using MailKit.Net.Smtp;
using MimeKit;

namespace Bislerium.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class ProfileController(
    IHttpContextAccessor contextAccessor,
    IGenericRepository<User> userRepository,
    IGenericRepository<UserRoles> userRoleRepository,
    IGenericRepository<Role> roleRepository,
    IGenericRepository<Blog> blogRepository,
    IGenericRepository<BlogImage> blogImageRepository,
    IGenericRepository<Comment> commentRepository,
    IGenericRepository<Reaction> reactionRepository,
    UserManager<User> userManager)
    : Controller
{

    [HttpGet("profile-details")]
    public IActionResult GetProfileDetails()
    {
       var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);

        var user = userRepository.GetById(userId);

        var userRole = userRoleRepository.GetAll().FirstOrDefault(x => x.UserId == user!.Id);

        var role = roleRepository.GetById(userRole!.RoleId);

        var result = new ProfileDetailsRecord()
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

        return Ok(result);
    }

    [HttpPatch("update-profile-details")]
    public IActionResult UpdateProfileDetails(ProfileDetailsRecord profileDetails)
    {
        var user = userRepository.GetById(profileDetails.UserId);

        user!.Name = profileDetails.FullName;
        user.PhoneNumber = profileDetails.MobileNumber;
        user.Email = profileDetails.EmailAddress;

        userRepository.Update(user);

        return Ok(true);
    }

    [HttpDelete("delete-profile")]
    public IActionResult DeleteProfile()
    {
       var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Guid.TryParse(userIdClaimValue, out var userId);

        var user = userRepository.GetById(userId);

        var blogs = blogRepository.GetAll().Where(x => x.CreatedBy == user.Id);

        var blogImages = blogImageRepository.GetAll().Where(x => blogs.Select(z => z.Id).Contains(x.BlogId));

        var comments = commentRepository.GetAll().Where(x => x.CreatedBy == user.Id);

        var reactions = reactionRepository.GetAll().Where(x => x.CreatedBy == user.Id);

        reactionRepository.Delete(reactions);
        commentRepository.Delete(comments);
        blogImageRepository.Delete(blogImages);
        blogRepository.Delete(blogs);
        userRepository.Delete(user);

        return Ok(true);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRecord changePassword)
    {
       var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        Guid.TryParse(userIdClaimValue, out var userId);

        var user = userRepository.GetById(userId);

        var validPassword = await userManager.CheckPasswordAsync(user!, changePassword.CurrentPassword);

        if (!validPassword) return BadRequest(false);

        if (changePassword.ConfirmPassword != changePassword.NewPassword) return BadRequest(false);

        var result = await userManager.ChangePasswordAsync(user!, changePassword.CurrentPassword, changePassword.NewPassword);

        if (result.Succeeded)
        {
            return Ok(true);
        }

        return BadRequest(false);
    }

    [HttpPost("reset-password")]
    public Task<IActionResult> ResetPassword(string emailAddress)
    {
        var user = userRepository.GetAll().FirstOrDefault(x => x.Email == emailAddress);

        if (user == null)
        {
            return Task.FromResult<IActionResult>(BadRequest(false));
        }

        var passwordHash = new PasswordHasher<User>();

        user.PasswordHash = passwordHash.HashPassword(user, "HelloWorld@123");

        userRepository.Update(user);

        const string senderEmail = "bisleriumblog@gmail.com";

        const string password = "hkas exio sois xjay";

        // Recipient's email address
        var recipientEmail = emailAddress;

        // Create a new MimeMessage
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Sender Name", senderEmail));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = $"Hello {user.Name}, <br><br> " +
                          $"This is to inform you that your password has been successfully reset as per your request. " +
                          $"Your new password is HelloWorld@123.<br><br>" +
                          $"Best regards,<br>" +
                          $"Bislerium.";

        // Create a body part
        var bodyBuilder = new BodyBuilder();
        bodyBuilder.TextBody = "This is a test email from C#";

        // Set the body message
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            // Connect to the SMTP server
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.example.com", 587, false);
                client.Authenticate(senderEmail, password);

                // Send the message
                client.Send(message);

                // Disconnect from the SMTP server
                client.Disconnect(true);
            }

            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to send email: " + ex.Message);
        }

        return Task.FromResult<IActionResult>(Ok(true));
    }
}