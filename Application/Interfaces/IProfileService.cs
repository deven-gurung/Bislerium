using Application.DTOs.Profile;
using Application.Interfaces.Base;

namespace Application.Interfaces;

public interface IProfileService
{
    ProfileDetailsDto GetProfileDetails();

    bool UpdateProfileDetails(ProfileDetailsDto profileDetails);

    bool DeleteProfile();

    Task<bool> ChangePassword(ChangePasswordDto changePassword);

    Task<bool> ResetPassword(string emailAddress);
}