using System.Security.Cryptography;
using System.Text;
using DatingDAL;
using DatingModels.Entities;
using DatingServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Controllers
{
    [Authorize] // This makes all endpoints in this controller require authorization by default
    public class AppUser(DataContext dataContext, IUserService userService) : BaseController
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

        [Authorize(Policy ="RequireAdminRole")]
        [HttpPost]
        public async Task<ActionResult<DatingModels.Entities.AppUser>> AddUser([FromBody]DatingModels.DTOs.AppUserDTO user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is null");
                }

                //check if user exists
                if(await userService.UserExistsAsync(user))
                {
                    return BadRequest("User already exists");
                }

                var result = await userService.AddDataUser(user);
                if (!result)
                {
                    return StatusCode(500, "Failed to create user");
                }
                
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
