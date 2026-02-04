using System.Runtime.InteropServices.Marshalling;

namespace AuthServiceIN6BV.Application.DTOs;

public class UserResponseDto
{
    public string id {get; set;} = string.Empty;
    public string Id { get; internal set; }
    public string Name {get; set;} = string.Empty;

    public string Surname {get; set;} = string.Empty;

    public string Username {get; set;} = string.Empty;

    public string Email  {get; set;} = string.Empty;

    public string ProfilePicture {get; set;} = string.Empty;

    public string Phone {get; set;} = string.Empty;

    public string Role {get; set;} = string.Empty;

    public bool Status {get; set;}

    public bool IsEmailVerifid {get; set;}

    public DateTime CreateAt {get; set;}

    public DateTime UpdateAt {get; set;}


}