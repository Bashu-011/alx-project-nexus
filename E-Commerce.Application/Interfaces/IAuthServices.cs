using E_Commerce.Application.DTOs.Auth;

namespace E_Commerce.Application.Interfaces;

//interface for authentication services - contains b/s logic for registering and logging in users
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterationRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}
