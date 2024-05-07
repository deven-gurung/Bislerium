﻿using Application.DTOs.Account;
using Application.DTOs.Dashboard;
using Application.DTOs.User;

namespace Application.Interfaces;

public interface IAdminService
{
    List<UserDetailDto> GetAllUsers();

    bool RegisterAdmin(RegisterDto register);

    DashboardDetailsDto GetDashboardDetails();
}