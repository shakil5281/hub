using ERP.Web.Areas.Admin.ViewModels;
using ERP.Web.Core.Entities.Security;
using ERP.Web.Core.Interfaces.Services;
using ERP.Web.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Infrastructure.Services;

public class RoleManagementService : IRoleManagementService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppDbContext _context;

    public RoleManagementService(RoleManager<ApplicationRole> roleManager, AppDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IReadOnlyList<RoleListItemVm>> GetAllAsync()
    {
        var roles = await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        var result = new List<RoleListItemVm>();

        foreach (var role in roles)
        {
            var permissionCount = await _context.RolePermissions.CountAsync(rp => rp.RoleId == role.Id);
            result.Add(new RoleListItemVm
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Description = role.Description,
                PermissionCount = permissionCount
            });
        }

        return result;
    }

    public async Task<RoleEditVm?> GetForEditAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return null;

        var selectedIds = await _context.RolePermissions
            .Where(rp => rp.RoleId == role.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        return new RoleEditVm
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description,
            PermissionIds = selectedIds
        };
    }

    public async Task<(bool Success, string? Error)> CreateAsync(RoleCreateVm model)
    {
        if (await _roleManager.RoleExistsAsync(model.Name))
            return (false, "Role name already exists.");

        var role = new ApplicationRole
        {
            Name = model.Name.Trim(),
            Description = model.Description?.Trim()
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        await SyncRolePermissionsAsync(role.Id, model.PermissionIds);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> UpdateAsync(RoleEditVm model)
    {
        var role = await _roleManager.FindByIdAsync(model.Id);
        if (role == null) return (false, "Role not found.");

        role.Name = model.Name.Trim();
        role.Description = model.Description?.Trim();

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        await SyncRolePermissionsAsync(role.Id, model.PermissionIds);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return (false, "Role not found.");

        if (role.Name == "Admin")
            return (false, "The Admin role cannot be deleted.");

        var permissions = await _context.RolePermissions.Where(rp => rp.RoleId == id).ToListAsync();
        _context.RolePermissions.RemoveRange(permissions);
        await _context.SaveChangesAsync();

        var result = await _roleManager.DeleteAsync(role);
        return result.Succeeded
            ? (true, null)
            : (false, string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task<IReadOnlyList<PermissionItemVm>> GetAllPermissionsAsync()
    {
        return await _context.Permissions
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Name)
            .Select(p => new PermissionItemVm
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Module = p.Module
            })
            .ToListAsync();
    }

    private async Task SyncRolePermissionsAsync(string roleId, IList<int> permissionIds)
    {
        var existing = await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        _context.RolePermissions.RemoveRange(existing);

        foreach (var permissionId in permissionIds.Distinct())
        {
            _context.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            });
        }

        await _context.SaveChangesAsync();
    }
}
