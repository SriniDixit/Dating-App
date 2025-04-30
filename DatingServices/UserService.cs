using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using DatingDAL;
using DatingModels.DTOs;
using DatingModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingServices;

public interface IUserService
{
    public Task<bool> UserExistsAsync(AppUserDTO appUserDTO);

    public Task<bool> AddDataUser(AppUserDTO appUserDTO);

    public Task<AppUser?> AuthenticateUser(AppUserDTO appUserDTO);
}

public class UserService(DataContext dataContext) : IUserService
{
    public async Task<bool> UserExistsAsync(AppUserDTO appUserDTO)
    {
        if (appUserDTO == null || string.IsNullOrWhiteSpace(appUserDTO.UserName))
            throw new ArgumentException("Invalid user data", nameof(appUserDTO));

        return await dataContext.Users.AnyAsync(_user => _user != null && 
            EF.Functions.Collate(_user.UserName, "NOCASE") == appUserDTO.UserName.Trim());
    }

    public async Task<bool> AddDataUser(AppUserDTO appUserDTO)
    {
        try
        {
            // computing hash
            using var hmac=new HMACSHA512();
            var passwordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(appUserDTO.Password));
            var passwordSalt=hmac.Key;

            var user = new AppUser
            {
                UserName = appUserDTO.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = appUserDTO.Role ?? "user"
            };

            await dataContext.Users.AddAsync(user);
            await dataContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    
    public async Task<AppUser?> AuthenticateUser(AppUserDTO appUserDTO)
    {
        try
        {
            var user = await dataContext.Users.FirstOrDefaultAsync(x => 
                EF.Functions.Collate(x.UserName, "NOCASE") == appUserDTO.UserName.Trim());

            if (user == null) return null;

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(appUserDTO.Password));

            return ConstantTimeArrayEqual(computedHash, user.PasswordHash) ? user : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    private bool ConstantTimeArrayEqual(byte[] a, byte[] b)
    {
        if (a == null || b == null || a.Length != b.Length)
            return false;
            
        var result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }

}
