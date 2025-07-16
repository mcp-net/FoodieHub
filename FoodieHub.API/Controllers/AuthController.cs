using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodieHub.API.Models.DTO;
using FoodieHub.API.Repositories;

namespace FoodieHub.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _tokenRepository = tokenRepository;
        }

        // POST: /api/Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                // Add roles to this User
                if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered! Please login.");
                    }
                }
            }

            return BadRequest("Something went wrong");
        }

        // POST: /api/Auth/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return BadRequest("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var (accessToken, refreshToken) =
                _tokenRepository.CreateTokens(user, roles.ToList());

            return Ok(new LoginResponseDto
            {
                JwtToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        // POST: /api/Auth/Refresh
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(
            [FromBody] RefreshRequestDto dto)
        {
            var stored = await _tokenRepository.GetRefreshTokenAsync(dto.RefreshToken);
            if (stored == null || stored.IsRevoked || stored.Expires < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            // Revoke old and issue new
            await _tokenRepository.RevokeRefreshTokenAsync(dto.RefreshToken);

            var user = await _userManager.FindByIdAsync(stored.UserId);
            var roles = await _userManager.GetRolesAsync(user!);
            var (newAccess, newRefresh) =
                _tokenRepository.CreateTokens(user!, roles.ToList());

            return Ok(new RefreshResponseDto
            {
                JwtToken = newAccess,
                RefreshToken = newRefresh
            });
        }
    }




    //////[Route("api/[controller]")]
    //////[ApiController]
    //////public class AuthController : ControllerBase
    //////{
    //////    private readonly UserManager<IdentityUser> _userManager;
    //////    private readonly ITokenRepository _tokenRepository;

    //////    public AuthController(UserManager<IdentityUser> userManager,
    //////        ITokenRepository tokenRepository)
    //////    {
    //////        _userManager = userManager;
    //////        _tokenRepository = tokenRepository;
    //////    }

    //////    // POST: /api/Auth/Register
    //////    [HttpPost]
    //////    [Route("Register")]
    //////    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    //////    {
    //////        var identityUser = new IdentityUser
    //////        {
    //////            UserName = registerRequestDto.Username,
    //////            Email = registerRequestDto.Username
    //////        };

    //////        var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

    //////        if (identityResult.Succeeded)
    //////        {
    //////            // Add roles to this User
    //////            if (registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
    //////            {
    //////                identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);

    //////                if (identityResult.Succeeded)
    //////                {
    //////                    return Ok("User was registered! Please login.");
    //////                }
    //////            }
    //////        }

    //////        return BadRequest("Something went wrong");
    //////    }

    //////    // POST: /api/Auth/Login
    //////    [HttpPost]
    //////    [Route("Login")]
    //////    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    //////    {
    //////        var user = await _userManager.FindByEmailAsync(loginRequestDto.Username);

    //////        if (user != null)
    //////        {
    //////            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

    //////            if (checkPasswordResult)
    //////            {
    //////                // Get Roles for this user
    //////                var roles = await _userManager.GetRolesAsync(user);

    //////                if (roles != null)
    //////                {
    //////                    // Create Token JWE
    //////                    var jwtToken = _tokenRepository.CreateJWEToken(user, roles.ToList());

    //////                    // old JWT Token creation
    //////                    //var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

    //////                    var response = new LoginResponseDto
    //////                    {
    //////                        JwtToken = jwtToken
    //////                    };

    //////                    return Ok(response);
    //////                }
    //////            }
    //////        }

    //////        return BadRequest("Username or password incorrect");
    //////    }
    //////}
}