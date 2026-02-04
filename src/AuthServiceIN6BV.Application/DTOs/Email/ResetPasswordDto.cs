using System.ComponentModel.DataAnnotations;
namespace AuthServiceIN6BV.Application.DTOs.Email;

public class ResetPasswordDto
{
    [Required]
    [MinLength (8)]
    public string Token {get; set;} = string.Empty;
    public string NewPassword { get; internal set; } = string.Empty;
}
