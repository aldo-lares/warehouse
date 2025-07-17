using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WarehouseAPI.Data;
using WarehouseAPI.DTOs;
using WarehouseAPI.Services;

namespace WarehouseAPI.Tests;

public class AuthServiceTests
{
    private ApplicationDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private IConfiguration GetConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "your-secret-key-here-make-it-long-and-secure-at-least-32-characters"},
            {"JwtSettings:Issuer", "WarehouseAPI"},
            {"JwtSettings:Audience", "WarehouseAPI"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = new Mock<ILogger<AuthService>>();
        var authService = new AuthService(context, config, logger.Object);

        var loginRequest = new LoginRequest
        {
            Email = "admin@warehouse.com",
            Password = "admin123"
        };

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("admin@warehouse.com", result.User.Email);
        Assert.Contains("Admin", result.User.Roles);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = new Mock<ILogger<AuthService>>();
        var authService = new AuthService(context, config, logger.Object);

        var loginRequest = new LoginRequest
        {
            Email = "admin@warehouse.com",
            Password = "wrongpassword"
        };

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = new Mock<ILogger<AuthService>>();
        var authService = new AuthService(context, config, logger.Object);

        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@warehouse.com",
            Password = "anypassword"
        };

        // Act
        var result = await authService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = new Mock<ILogger<AuthService>>();
        var authService = new AuthService(context, config, logger.Object);

        // Act
        var result = await authService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("admin@warehouse.com", result.Email);
        Assert.Contains("Admin", result.Roles);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryContext();
        var config = GetConfiguration();
        var logger = new Mock<ILogger<AuthService>>();
        var authService = new AuthService(context, config, logger.Object);

        // Act
        var result = await authService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}