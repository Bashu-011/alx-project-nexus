using E_Commerce.Core.Entities;

namespace E_Commerce.Application.DTOs.Auth;

public class RegisterationRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Customer;

}
