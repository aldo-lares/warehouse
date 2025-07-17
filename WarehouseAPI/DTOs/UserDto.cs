namespace WarehouseAPI.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
    public DateTime CreatedAt { get; set; }
}