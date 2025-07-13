using UserManagementAPI.Domain.Entities;

namespace UserManagementAPI.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<int> GetTotalCountAsync(string? search = null);
    }
}
