using AuthServiceIN6BV.Application.DTOs;
using AuthServiceIN6BV.Application.DTOs.Email;

namespace AuthServiceIN6BV.Application.Interface;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);

    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);

    Task<EmailResponseDto> VerifyEmailAsync (VerifyEmailDto verifyEmailDto);
    Task <EmailResponseDto> ResendVerificationEmailAsync(ResendVerificationDto resendVerificationDto);
    Task <EmailResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task <EmailResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<UserResponseDto> GetUserByAsync(string userId);

    
     
}