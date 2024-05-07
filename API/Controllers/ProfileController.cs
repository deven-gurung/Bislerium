using System.Net;
using Application.DTOs.Profile;
using Application.Interfaces;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController(IProfileService profileService) : Controller
{
    [HttpGet("profile-details")]
    public IActionResult GetProfileDetails()
    {
        var result = profileService.GetProfileDetails();

        return Ok(new ResponseDto<ProfileDetailsDto>()
        {
            Message = "Successfully Fetched",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1,
            Status = "Success",
            Data = result
        });
    }

    [HttpPatch("update-profile-details")]
    public IActionResult UpdateProfileDetails(ProfileDetailsDto profileDetails)
    {
        var result = profileService.UpdateProfileDetails(profileDetails);

        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully Updated",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1,
            Status = "Success",
            Data = result 
        });
    }

    [HttpDelete("delete-profile")]
    public IActionResult DeleteProfile()
    {
        var result = profileService.DeleteProfile();

        return Ok(new ResponseDto<bool>()
        {
            Message = "Successfully Deleted",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1,
            Status = "Success",
            Data = result 
        });
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePassword)
    {
        var result = await profileService.ChangePassword(changePassword);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Successfully Updated",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1,
                Status = "Success",
                Data = true
            });
        }

        return BadRequest(new ResponseDto<object>()
        {
            Message = "Password not valid",
            StatusCode = HttpStatusCode.BadRequest,
            TotalCount = 1,
            Status = "Invalid",
            Data = false
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(string emailAddress)
    {
        var result = await profileService.ResetPassword(emailAddress);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Mail Successfully Triggered",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1,
                Status = "Success",
                Data = true
            });
        }

        return BadRequest(new ResponseDto<object>()
        {
            Message = "Mail Could not be triggered",
            StatusCode = HttpStatusCode.BadRequest,
            TotalCount = 1,
            Status = "Invalid",
            Data = false
        });
    }
}