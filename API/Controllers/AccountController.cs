using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Account;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Utilities;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bislerium.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService accountService, IOptions<JWTSettings> jwtSettings) : Controller
{
    private readonly JWTSettings _jwtSettings = jwtSettings.Value;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginRequest)
    {
        var user = accountService.GetUserByEmail(loginRequest.EmailAddress);

        if (user == new User())
        {
            return NotFound(new ResponseDto<bool>()
            {
                Message = "User not found",
                Data = false,
                Status = "Not Found",
                StatusCode = HttpStatusCode.NotFound,
                TotalCount = 0
            });
        }

        var isPasswordValid = await accountService.IsPasswordValid(user.Email!, loginRequest.Password);

        if (!isPasswordValid)
        {
            return Unauthorized(new ResponseDto<bool>()
            {
                Message = "Password incorrect",
                Data = false,
                Status = "Unauthorized",
                StatusCode = HttpStatusCode.Unauthorized,
                TotalCount = 0
            });
        }

        var roleId = accountService.GetUserRoleId(user.Email ?? "");

        var role = accountService.GetRoleById(roleId);
        
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, (user.Id.ToString() ?? null) ?? string.Empty),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Role, role.Name ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var symmetricSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.DurationInMinutes));

        var accessToken = new JwtSecurityToken(
           _jwtSettings.Issuer,
           _jwtSettings.Audience,
           claims: authClaims,
           signingCredentials: signingCredentials,
           expires: expirationTime
        );

        var userDetails = new UserDto()
        {
            Id = user.Id,
            Name = user.Name,
            Username = user.UserName ?? "",
            EmailAddress = user.Email ?? "",
            RoleId = role.Id,
            Role = role.Name ?? "",
            ImageUrl = user.ImageURL ?? "dummy.svg",
            Token = new JwtSecurityTokenHandler().WriteToken(accessToken)
        };

        return Ok(new ResponseDto<UserDto>()
        {
            Message = "Successfully authenticated",
            Data = userDetails,
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1
        });
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterDto register)
    {
        var existingUser = accountService.GetExistingUser(register.EmailAddress, register.Username);

        if (existingUser.Id == Guid.Empty)
        {
            var passwordHash = new PasswordHasher<User>();
            
            var appUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = register.FullName,
                Email = register.EmailAddress,
                UserName = register.Username,
                PhoneNumber = register.MobileNumber,
                ImageURL = register.ImageURL
            };

            appUser.PasswordHash = passwordHash.HashPassword(appUser, register.Password);
            
            accountService.InsertUser(appUser);

            return Ok(new ResponseDto<object>()
            {
                Message = "Successfully registered",
                Data = true,
                Status = "Success",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1
            });
        }

        return BadRequest(new ResponseDto<bool>()
        {
            Message = "Existing user with the same user name or email address",
            Data = false,
            Status = "Bad Request",
            StatusCode = HttpStatusCode.BadRequest,
            TotalCount = 0
        });
    }
}