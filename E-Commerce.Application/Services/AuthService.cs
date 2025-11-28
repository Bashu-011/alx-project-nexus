using AutoMapper;
using E_Commerce.Application.DTOs.Auth;
using E_Commerce.Application.Interfaces;
using E_Commerce.Core.Entities;
using E_Commerce.Core.Interfaces;

namespace E_Commerce.Application.Services;

// Service for handling user authentication 

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly JwtTokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        JwtTokenService tokenService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterationRequest request)
    {
        //check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        //creates a new user
        var user = _mapper.Map<User>(request);
        //hasing password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Role = request.Role;
        user.IsActive = true;

        await _userRepository.AddAsync(user);

        //generates token
        var token = _tokenService.GenerateToken(user);
        var response = _mapper.Map<AuthResponse>(user);
        response.Token = token;
        response.ExpiresAt = _tokenService.GetTokenExpiration();
        response.Role = user.Role.ToString();

        return response;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        //finds user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is inactive");
        }

        //generates token
        var token = _tokenService.GenerateToken(user);
        var response = _mapper.Map<AuthResponse>(user);
        response.Token = token;
        response.ExpiresAt = _tokenService.GetTokenExpiration();

        return response;
    }
}