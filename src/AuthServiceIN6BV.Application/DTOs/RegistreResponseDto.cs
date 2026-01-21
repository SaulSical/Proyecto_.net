namespace AuthServiceIN6BV.Application.DTOs;

public class RegisterResponseDto
{
    public bool Success {get; set;}

    public UserResponseDto User {get; set;} = null;

    public string Message {get; set;} = string.Empty;

    public bool EmailVerificationRequired {get; set;} = true;
}