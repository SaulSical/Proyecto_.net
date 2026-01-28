using AuthServiceIN6BV.Domain.Entities;
using AuthServiceIN6BV.Domain.Interface;
using AuthServiceIN6BV.Persistence.Data;
using Microsoft.EntityFrameworkCore;
 
namespace AuthServiceIN6BV.Persistence.Repositories;
 
public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
 
    }
 
    public async Task<int> CountUserInRoleAsync(string roleName)
    {
        return await context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur =>ur.Role.Name ==roleName)
            .Select(Uri => Uri.UserId)
            .Distinct()
            .CountAsync();
    }
 
    public async Task<IReadOnlyList<User>> GetUserByRoleAsync(string roleName)
    {
        var users = await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
            .ToListAsync();
        return users;
    }
 
    public async Task <IReadOnlyList<string>> GetUserRoleNameAsync(string userId)
    {
        var roles = await context.UserRoles
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(Uri => Uri.Role.Name)
            .ToListAsync();
        return roles;
    }
}