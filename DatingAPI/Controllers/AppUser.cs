using DatingDAL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUser(DataContext dataContext) : ControllerBase
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
        public async Task<ActionResult<DatingModels.Entities.AppUser>> AddUser(DatingModels.Entities.AppUser user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("User data is null");
                }

                var existingUser = await dataContext.Users
                    .FirstOrDefaultAsync(u => u.Id == user.Id);
                    
                if (existingUser != null)
                {
                    return BadRequest("UserId already exists");
                }

                await dataContext.Users.AddAsync(user);
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
