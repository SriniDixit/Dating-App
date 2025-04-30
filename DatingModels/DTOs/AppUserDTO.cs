using System;

namespace DatingModels.DTOs;

public class AppUserDTO
{
    public required string UserName { get; set; }

    public required string Password { get; set; }

    public string Role { get; set; } = "user";
}
