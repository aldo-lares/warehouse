using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using WarehouseAPI.DTOs;

namespace WarehouseAPI.Tests;

public class AuthorizationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthorizationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetInventory_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/warehouse/inventory");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetInventory_WithValidToken_ReturnsInventory()
    {
        // Arrange
        var token = await GetValidTokenAsync("admin@warehouse.com", "admin123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/warehouse/inventory");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("items", content.ToLower());
    }

    [Fact]
    public async Task GetAdminStats_WithAdminToken_ReturnsStats()
    {
        // Arrange
        var token = await GetValidTokenAsync("admin@warehouse.com", "admin123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/warehouse/admin/stats");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("totalitems", content.ToLower());
    }

    [Fact]
    public async Task GetAdminStats_WithUserToken_ReturnsForbidden()
    {
        // Arrange
        var token = await GetValidTokenAsync("user@warehouse.com", "user123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/warehouse/admin/stats");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AddInventoryItem_WithUserToken_ReturnsCreated()
    {
        // Arrange
        var token = await GetValidTokenAsync("user@warehouse.com", "user123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var newItem = new { Name = "Test Item", Quantity = 10, Location = "T1" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/warehouse/inventory", newItem);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AddInventoryItem_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange
        var newItem = new { Name = "Test Item", Quantity = 10, Location = "T1" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/warehouse/inventory", newItem);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInventoryItem_WithAdminToken_ReturnsNoContent()
    {
        // Arrange
        var token = await GetValidTokenAsync("admin@warehouse.com", "admin123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/warehouse/inventory/1");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteInventoryItem_WithUserToken_ReturnsForbidden()
    {
        // Arrange
        var token = await GetValidTokenAsync("user@warehouse.com", "user123");
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/warehouse/inventory/1");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<string> GetValidTokenAsync(string email, string password)
    {
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return loginResponse?.Token ?? throw new InvalidOperationException("Failed to get token");
    }
}