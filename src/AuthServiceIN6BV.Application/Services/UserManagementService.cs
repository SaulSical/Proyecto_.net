using AuthServiceIN6BV.Application.DTOs;
using AuthServiceIN6BV.Application.Interfaces;
using AuthServiceIN6BV.Domain.Constans;
using AuthServiceIN6BV.Domain.Entities;
using AuthServiceIN6BV.Domain.Interface;

namespace AuthServiceIN6BV.Application.Services;

public class UserManagementService(IUserRepository users, IRoleRepository roles, ICloudinaryService cloudinary) : IUserManagementService
{
    public async Task<UserResponseDto> UpdateUserRoleAsync(string userId, string roleName)
    {
        // Normalize
        roleName = roleName?.Trim().ToUpperInvariant() ?? string.Empty;

        // Validate inputs
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("Invalid userId", nameof(userId));
        if (!RoleConstans.AllWedRoles.Contains(roleName))
            throw new InvalidOperationException($"Role not allowed. Use {RoleConstans.ADMIN_ROLE} or {RoleConstans.USER_ROLE}");

        // Load user with roles
        var user = await users.GetByIdAsync(userId);

        // If demoting an admin, prevent removing last admin
        var isUserAdmin = user.UserRoles.Any(r => r.Role.Name == RoleConstans.ADMIN_ROLE);
        if (isUserAdmin && roleName != RoleConstans.ADMIN_ROLE)
        {
            var adminCount = await roles.CountUserInRoleAsync(RoleConstans.ADMIN_ROLE);

            if (adminCount <= 1)
            {
                throw new InvalidOperationException("Cannot remove the last administrator");
            }
        }

        // Find role entity
        var role = await roles.GetByNameAsync(roleName)
                       ?? throw new InvalidOperationException($"Role {roleName} not found");

        // Update role using repository method
        await users.UpdateUserRoleAsync(userId, role.Id);

        // Reload user with updated roles
        user = await users.GetByIdAsync(userId);

        // Map to response
        return new UserResponseDto
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.SurName,
            Username = user.UserName,
            Email = user.Email,
            ProfilePicture = cloudinary.GetFullimageUrl(user.UserProfile?.ProfilePicture ?? string.Empty),
            Phone = user.UserProfile?.Phone ?? string.Empty,
            Role = role.Name,
            Status = user.Status,
            IsEmailVerifid = user.UserEmail?.EmailVerified ?? false,
            CreateAt = user.CreatedAt,
            UpdateAt = user.UpdatedAt
        };
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId)
    {
        var roleNames = await roles.GetUserRoleNameAsync(userId);
        return roleNames;
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetUsersByRoleAsync(string roleName)
    {
        roleName = roleName?.Trim().ToUpperInvariant() ?? string.Empty;
        var usersInRole = await roles.GetUserByRoleAsync(roleName);
        return usersInRole.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Surname = u.SurName,
            Username = u.UserName,
            Email = u.Email,
            ProfilePicture = cloudinary.GetFullimageUrl(u.UserProfile?.ProfilePicture ?? string.Empty),
            Phone = u.UserProfile?.Phone ?? string.Empty,
            Role = roleName,
            Status = u.Status,
            IsEmailVerifid = u.UserEmail?.EmailVerified ?? false,
            CreateAt = u.CreatedAt,
            UpdateAt = u.UpdatedAt
        }).ToList();
    }

    public Task<IReadOnlyList<UserResponseDto>> GetUserByRoleAsync(string roleName)
    {
        throw new NotImplementedException();
    }
}
