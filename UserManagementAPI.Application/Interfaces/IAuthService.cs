using UserManagementAPI.Application.DTOs;

namespace UserManagementAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<string> GenerateTokenAsync(UserDto user);
        Task<UserDto?> ValidateTokenAsync(string token);
    }
}
