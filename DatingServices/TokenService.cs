using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingModels.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingServices;

public interface ITokenService
{
    public string GenerateToken(AppUserDTO appUserDTO);
}

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            config["TokenKey"] ?? throw new InvalidOperationException("TokenKey is not configured")));
    }

    public string GenerateToken(AppUserDTO appUserDTO)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, appUserDTO.UserName)
        };

        // Handle multiple roles if Role field contains comma-separated values
        if (!string.IsNullOrEmpty(appUserDTO.Role))
        {
            var roles = appUserDTO.Role.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "user"));
        }

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
