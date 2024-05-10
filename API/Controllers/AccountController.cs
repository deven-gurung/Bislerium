using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces;
using Application.Records;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController(UserManager<User> userManager, IGenericRepository<Role> roleRepository) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRecord loginRequest)
        {
            var user = await userManager.FindByEmailAsync(loginRequest.EmailAddress);

            if (user == null || !await userManager.CheckPasswordAsync(user, loginRequest.Password))
                return NotFound(false);

            var roles = await userManager.GetRolesAsync(user);
            var role = roleRepository.GetAll().FirstOrDefault(x => x.Name == roles.FirstOrDefault());

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email ?? ""),
                new(ClaimTypes.Role, role?.Name ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var symmetricSigningKey = new SymmetricSecurityKey("My$3cr3tK3yF0rJWT\n"u8.ToArray());
            var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

            var expirationTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(100));

            var accessToken = new JwtSecurityToken(
               "Bislerium",
               "Bislerium",
               claims: authClaims,
               expires: expirationTime,
               signingCredentials: signingCredentials
            );

            var userDetails = new UserRecord()
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.UserName ?? "",
                EmailAddress = user.Email ?? "",
                RoleId = role?.Id ?? Guid.NewGuid(),
                Role = role?.Name ?? "",
                ImageUrl = user.ImageURL ?? "dummy.svg",
                Token = new JwtSecurityTokenHandler().WriteToken(accessToken)
            };

            return Ok(userDetails);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRecord register)
        {
            var existingUser = await userManager.FindByEmailAsync(register.EmailAddress);
            if (existingUser != null || await userManager.FindByNameAsync(register.Username) != null)
                return BadRequest(false);

            var role = roleRepository.GetAll().FirstOrDefault(x => x.Name == "Blogger");

            var appUser = new User()
            {
                Id = Guid.NewGuid(),
                Name = register.FullName,
                Email = register.EmailAddress,
                UserName = register.Username,
                PhoneNumber = register.MobileNumber,
                ImageURL = register.ImageURL
            };

            var result = await userManager.CreateAsync(appUser, register.Password);
            if (!result.Succeeded)
                return BadRequest(false);

            await userManager.AddToRoleAsync(appUser, role?.Name ?? "");

            return Ok(true);
        }
    }
}
