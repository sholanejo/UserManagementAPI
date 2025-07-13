using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using UserManagementAPI.Application.DTOs;
using UserManagementAPI.Application.Interfaces;
using UserManagementAPI.Application.Services;
using UserManagementAPI.Domain.Entities;
using UserManagementAPI.Domain.Enums;
using UserManagementAPI.Domain.Exceptions;

namespace UserManagementAPI.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AuthService>>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup JWT configuration
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(x => x["Key"]).Returns("ThisIsASecretKeyForJWTTokenGenerationThatShouldBeAtLeast32CharactersLong");
            jwtSection.Setup(x => x["Issuer"]).Returns("UserManagementApi");
            jwtSection.Setup(x => x["Audience"]).Returns("UserManagementApi.Users");
            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSection.Object);

            _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@system.com",
                Password = "Admin123!"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@system.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                Status = UserStatus.Active,
                IsDeleted = false,
                LoginAttempts = 0
            };

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Status = user.Status
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockMapper.Setup(x => x.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(user.Id, result.User.Id);
            Assert.Equal(user.Email, result.User.Email);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ThrowsDomainException()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "admin@system.com",
                Password = "WrongPassword"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@system.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Status = UserStatus.Active,
                IsDeleted = false,
                LoginAttempts = 0
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(() => _authService.LoginAsync(loginDto));
        }

    }
}