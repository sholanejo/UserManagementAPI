using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagementAPI.Application.DTOs;
using UserManagementAPI.Application.Interfaces;
using UserManagementAPI.Application.Services;
using UserManagementAPI.Domain.Entities;
using UserManagementAPI.Domain.Exceptions;

namespace UserManagementAPI.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<IEventPublisher> _mockEventPublisher;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockEventPublisher = new Mock<IEventPublisher>();
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object, _mockEventPublisher.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUser_ReturnsUserDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsDeleted = false
            };
            var userDto = new UserDto
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingUser_ThrowsDomainException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _userService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task CreateAsync_ValidUser_ReturnsUserDto()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(createDto.Email))
                .ReturnsAsync((User?)null);
            _mockMapper.Setup(x => x.Map<User>(createDto))
                .Returns(user);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.Equal("jane.smith@example.com", result.Email);
        }

        [Fact]
        public async Task CreateAsync_DuplicateEmail_ThrowsDomainException()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "Password123!"
            };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "jane.smith@example.com"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(createDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _userService.CreateAsync(createDto));
        }
    }
}
