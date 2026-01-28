using AuthServiceIN6BV.Application.Services;
using AuthServiceIN6BV.Domain.Entities;
using AuthServiceIN6BV.Domain.Interface;
using AuthServiceIN6BV.Persistence.Data;
using Microsoft.EntityFrameworkCore;
 
namespace AuthServiceIN6BV.Persistence.Repositories;
 
 // repositorio de usuario
public class UserRepository(ApplicationDbContext context) : IUserRepository
{

    // obtener usuario por id
    public async Task<User>GetByIdAsync(String id)
    {
        var user = await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
            return user ?? throw new InvalidOperationException($"User with id {id} not found.");
    }
 
    // obtener usuario por email
    public async Task<User?>GetByEmailAsync(String email)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
    }
 
    // obtener usuario por username
    public async Task<User?>GetByUsernameAsync(String username)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.UserName, username));
    }
 
    // obtener usuario por token de verificacion de email
    public async Task<User?>GetByEmailVerificationTokenAsync(String Token)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserEmail != null &&
                                      u.UserEmail.EmailVerificationToken == Token &&
                                      u.UserEmail.EmailVerificationTokenExpiry > DateTime.UtcNow);
    }

    // obtener usuario por token de reseteo de password

        public async Task<User?>GetByPasswordResetTokenAsync(String Token)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserPasswordReset != null &&
                                      u.UserPasswordReset.PasswordResetToken == Token &&
                                      u.UserPasswordReset.PasswordResetTokenExpiry > DateTime.UtcNow);
    }

    // crear usuario
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }

    // actualizar usuario
    public async Task<User> UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);

    }

    // eliminar usuario
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await  GetByIdAsync(id);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    // verificar si existe usuario por email
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users
            .AnyAsync(u => EF.Functions.ILike(u.Email, email));
    }

    // verificar si existe usuario por username

    public async Task<bool> ExitsByUsernameAsync(string username)
    {
        return await context.Users
            .AnyAsync(u => EF.Functions.ILike(u.UserName, username));
    }

    // actualizar rol de usuario
    public async Task UpdateUserRoleAsync(string userId, string roleId)
    {
        var existingRoles = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        context.UserRoles.RemoveRange(existingRoles);

        var userRole = new UserRole
        {
            Id = UuidGenerator.GenerateUserId(),
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.UserRoles.Add(userRole);
        await context.SaveChangesAsync();
    }



}   