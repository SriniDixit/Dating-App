using System.Security.Cryptography;
using System.Text;
using DatingDAL;
using DatingModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Controllers
{
    public class AppUser(DataContext dataContext) : BaseController
    {
        private readonly DataContext dataContext = dataContext;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DatingModels.Entities.AppUser>>> GetUsers()
        {
            try
            {
                var users = await dataContext.Users.ToListAsync();
                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DatingModels.Entities.AppUser>> GetUserById(int id)
        {
            try
            {
                var user = await dataContext.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DatingModels.Entities.AppUser>> AddUser([FromBody]DatingModels.Entities.AppUserDTO user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is null");
                }

              
                // computing hash
                using var hmac=new HMACSHA512();
                var passwordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                var passwordSalt=hmac.Key;

                DatingModels.Entities.AppUser appUser=new DatingModels.Entities.AppUser(){
                    UserName=user.UserName,
                    PasswordHash=passwordHash,
                    PasswordSalt=passwordSalt
                };

                await dataContext.Users.AddAsync(appUser);
                await dataContext.SaveChangesAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
