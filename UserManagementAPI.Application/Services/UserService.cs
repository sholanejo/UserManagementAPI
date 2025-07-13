using AutoMapper;
using Microsoft.Extensions.Logging;
using UserManagementAPI.Application.DTOs;
using UserManagementAPI.Application.Interfaces;
using UserManagementAPI.Domain.Entities;
using UserManagementAPI.Domain.Events;
using UserManagementAPI.Domain.Exceptions;

namespace UserManagementAPI.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IEventPublisher _eventPublisher;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger,
            IEventPublisher eventPublisher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                throw new DomainException($"User with ID {id} not found");
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null)
        {
            var users = await _userRepository.GetAllAsync(page, pageSize, search);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            _logger.LogInformation("Creating new user with email: {Email}", dto.Email);

            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Attempt to create user with duplicate email: {Email}", dto.Email);
                throw new DomainException("Email address is already in use");
            }

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            var createdUser = await _userRepository.CreateAsync(user);

            var userCreatedEvent = new UserCreatedEvent
            {
                UserId = createdUser.Id,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                CreatedAt = createdUser.CreatedAt
            };

            await _eventPublisher.PublishAsync(userCreatedEvent);

            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);

            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Attempt to update non-existent user with ID: {UserId}", id);
                throw new DomainException($"User with ID {id} not found");
            }

            _mapper.Map(dto, user);
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User updated successfully with ID: {UserId}", id);

            return _mapper.Map<UserDto>(updatedUser);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Attempt to delete non-existent user with ID: {UserId}", id);
                throw new DomainException($"User with ID {id} not found");
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User soft deleted successfully with ID: {UserId}", id);
        }

        public async Task<UserDto> RestoreAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Attempt to restore non-existent user with ID: {UserId}", id);
                throw new DomainException($"User with ID {id} not found");
            }

            user.IsDeleted = false;
            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            var restoredUser = await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User restored successfully with ID: {UserId}", id);

            return _mapper.Map<UserDto>(restoredUser);
        }

        public async Task<int> GetTotalCountAsync(string? search = null)
        {
            return await _userRepository.GetTotalCountAsync(search);
        }
    }
}
