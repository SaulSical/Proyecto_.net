
using AuthServiceIN6BV.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace AuthServiceIN6BV.Application.DTOs;


public class RegisterDto
{
    [Required]
    [MaxLength(25)]
    public string Name {get; set;} = string.Empty;

    [Required]
    [MaxLength(25)]
    public string Surname {get; set;} = string.Empty;

    [Required]
    public string UserName {get; set;} = string.Empty;

    [Required]
    [EmailAddress]
    public string Email {get; set;} = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password {get; set;} = string.Empty;

    [Required]
    [StringLength(8, MinimumLength = 8)]
    public string Phome {get; set;} = string.Empty;

    
    public IFileData? ProfilePicture {get; set;}
}