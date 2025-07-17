using WarehouseAPI.DTOs;

namespace WarehouseAPI.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> GetUserByIdAsync(int userId);
}