using System;

namespace DatingModels.DTOs;

public class LoginUserDTO
{
    public required string UserName { get; set; }
    public required string Token { get; set; }
}
