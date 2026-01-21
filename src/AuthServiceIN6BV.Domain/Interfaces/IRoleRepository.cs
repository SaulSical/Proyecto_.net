using AuthServiceIN6BV.Domain.Entities;

namespace AuthServiceIN6BV.Domain.Interface;

public interface IRolRepository
{
    Task<Role> GetByNameAsync(string name);
    Task<int> CountUserInRoleAsync(string roleName);
    Task<IReadOnlyCollection<User>> GetUserByRoleAsync(string roleName);
    Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId);

}