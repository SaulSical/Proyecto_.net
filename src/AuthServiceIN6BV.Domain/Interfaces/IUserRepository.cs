using AuthServiceIN6BV.Domain.Entities;

namespace AuthServiceIN6BV.Domain.Interface;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> GetByIdAsync(String id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailVerificationTokenAsync(string token);
    Task<User> GetByPasswordResetTokenAsync(string token);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExitsByUsernameAsync(string username);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(string id);
    Task UpdateUserRoleAsync(string userId, string roleId);
}