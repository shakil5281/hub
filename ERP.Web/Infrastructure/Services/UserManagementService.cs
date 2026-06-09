using ERP.Web.Areas.Admin.ViewModels;
using ERP.Web.Core.DTOs;
using ERP.Web.Core.Entities.Security;
using ERP.Web.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUserService = currentUserService;
    }

    public Task<IReadOnlyList<UserListItemVm>> GetAllAsync() => SearchAsync(new UserFilterDto());

    public async Task<IReadOnlyList<UserListItemVm>> SearchAsync(UserFilterDto filter)
    {
        var companyId = _currentUserService.CompanyId;
        var query = _userManager.Users.Where(u => u.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(u =>
                (u.Email != null && u.Email.Contains(search)) ||
                u.FullName.Contains(search));
        }

        if (filter.IsActive == true)
            query = query.Where(u => !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);
        else if (filter.IsActive == false)
            query = query.Where(u => u.LockoutEnabled && u.LockoutEnd > DateTimeOffset.UtcNow);

        var users = await query.OrderBy(u => u.FullName).ToListAsync();
        HashSet<string>? roleUserIds = null;

        if (!string.IsNullOrWhiteSpace(filter.RoleName))
        {
            var roleUsers = await _userManager.GetUsersInRoleAsync(filter.RoleName.Trim());
            roleUserIds = roleUsers
                .Where(u => u.CompanyId == companyId)
                .Select(u => u.Id)
                .ToHashSet(StringComparer.Ordinal);
        }

        var result = new List<UserListItemVm>();
        foreach (var user in users)
        {
            if (roleUserIds != null && !roleUserIds.Contains(user.Id))
                continue;

            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserListItemVm
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsActive = !user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
            });
        }

        return result;
    }

    public async Task<UserEditVm?> GetForEditAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.CompanyId != _currentUserService.CompanyId) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserEditVm
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            RoleName = roles.FirstOrDefault() ?? string.Empty,
            IsActive = !user.LockoutEnabled || user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.UtcNow
        };
    }

    public async Task<(bool Success, string? Error)> CreateAsync(UserCreateVm model)
    {
        if (!await _roleManager.RoleExistsAsync(model.RoleName))
            return (false, "Selected role does not exist.");

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            EmailConfirmed = true,
            FullName = model.FullName.Trim(),
            CompanyId = _currentUserService.CompanyId
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, model.RoleName);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(UserEditVm model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null || user.CompanyId != _currentUserService.CompanyId)
            return (false, "User not found.");

        user.Email = model.Email.Trim();
        user.UserName = model.Email.Trim();
        user.FullName = model.FullName.Trim();

        if (model.IsActive)
        {
            user.LockoutEnd = null;
            user.LockoutEnabled = false;
        }
        else
        {
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        if (!string.IsNullOrWhiteSpace(model.RoleName))
        {
            if (!await _roleManager.RoleExistsAsync(model.RoleName))
                return (false, "Selected role does not exist.");
            await _userManager.AddToRoleAsync(user, model.RoleName);
        }

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.CompanyId != _currentUserService.CompanyId)
            return (false, "User not found.");

        if (user.Email == _currentUserService.UserName)
            return (false, "You cannot delete your own account.");

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<(bool Success, string? Error)> ResetPasswordAsync(string id, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.CompanyId != _currentUserService.CompanyId)
            return (false, "User not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
