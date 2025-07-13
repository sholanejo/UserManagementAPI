using UserManagementAPI.Application.DTOs;

namespace UserManagementAPI.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto);
        Task DeleteAsync(Guid id);
        Task<UserDto> RestoreAsync(Guid id);
        Task<int> GetTotalCountAsync(string? search = null);
    }
}
