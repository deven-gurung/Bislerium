using Domain.Entities.Identity;

namespace Application.Interfaces;

public interface IAccountService
{
    User GetUserByEmail(string email);

    Guid GetUserRoleId(string email);

    Task<bool>  IsPasswordValid(string email, string password);

    Role GetRoleById(Guid roleId);

    User GetExistingUser(string emailAddress, string username);

    Role GetRoleByName(string roleName);

    void InsertUser(User user);
}