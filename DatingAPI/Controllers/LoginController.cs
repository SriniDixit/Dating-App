using DatingModels.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DatingServices;

namespace DatingAPI.Controllers
{
    public class LoginController(IUserService userService, ITokenService tokenService) : BaseController
    {
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost]
        public async Task<ActionResult<LoginUserDTO>> Authenticate(AppUserDTO appUserDTO)
        {
            try
            {
                if (appUserDTO == null)
                    return BadRequest("Invalid user data");

                var user = await _userService.AuthenticateUser(appUserDTO);
                if (user == null)
                    return Unauthorized();
                
                // Set the role from the authenticated user
                appUserDTO.Role = user.Role;
                
                var loginUserDTO = new LoginUserDTO
                {
                    UserName = appUserDTO.UserName,
                    Token = _tokenService.GenerateToken(appUserDTO)
                };
                
                return loginUserDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
