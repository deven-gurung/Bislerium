using System.Net;
using Application.DTOs.Account;
using Application.DTOs.Dashboard;
using Application.DTOs.User;
using Application.Interfaces;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bislerium.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AdminController(IAdminService adminService) : Controller
{
    [HttpGet("get-all-users")]
    public IActionResult GetAllUsers()
    {
        var users = adminService.GetAllUsers();

        return Ok(new ResponseDto<List<UserDetailDto>>()
        {
            Message = "Successfully Retrieved",
            Data = users,
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1
        });
    }

    [HttpPost("register-admin")]
    public IActionResult RegisterAdmin(RegisterDto register)
    {
        var result = adminService.RegisterAdmin(register);

        if (result)
        {
            return Ok(new ResponseDto<object>()
            {
                Message = "Admin Registered Successfully",
                Data = true,
                Status = "Success",
                StatusCode = HttpStatusCode.OK,
                TotalCount = 1
            });
        }
        else
        {
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

    [HttpGet("dashboard-details")]
    public IActionResult GetDashboardDetails()
    {
        var dashboardDetails = adminService.GetDashboardDetails();

        var result = new ResponseDto<DashboardDetailsDto>()
        {
            Message = "Success",
            StatusCode = HttpStatusCode.OK,
            TotalCount = 1,
            Data = dashboardDetails,
            Status = "Success"
        };

        return Ok(result);
    }
}