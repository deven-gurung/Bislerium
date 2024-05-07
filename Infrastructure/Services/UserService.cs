using System.Security.Claims;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class UserService(IHttpContextAccessor contextAccessor) : IUserService
{
    public Guid UserId
    {
        get
        {
            var userIdClaimValue = contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdClaimValue, out var userId) ? userId : Guid.NewGuid();
        }
    }
}