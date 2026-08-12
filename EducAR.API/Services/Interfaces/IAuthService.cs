using EducAR.API.DTOs.Auth;

namespace EducAR.API.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> Login(LoginRequestDto request);
}