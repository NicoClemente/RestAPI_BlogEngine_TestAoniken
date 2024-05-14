using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RestAPi_BlogEngine_TestAoniken.Exceptions;
using RestAPi_BlogEngine_TestAoniken.Models;
using RestAPi_BlogEngine_TestAoniken.Repositories;
using RestAPi_BlogEngine_TestAoniken.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RestAPi_BlogEngine_TestAoniken.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService; // Service for authentication-related operations
        private readonly IUserRepository _userRepository; // Repository for user-related data operations


        // Constructor to initialize the controller with the necessary services and repositories
        public AccountController(AuthService authService, IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        //Account Endpoints

        [HttpPost("register")]
        [SwaggerOperation(
            Summary = "Register a new user",
            Description = "This endpoint allows a new user to register by providing a username, password, and role."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "If the user was registered successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the username, password, or role is missing or invalid.")]
        public IActionResult Register([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Role))
            {
                throw new ApiException("Username, password, and role are required.", (int)HttpStatusCode.BadRequest);
            }

            var existingUser = _userRepository.GetUserByUsername(user.Username);

            if (existingUser != null)
            {
                throw new ApiException("Username is already taken.", (int)HttpStatusCode.BadRequest);
            }

            _userRepository.AddUser(user);
            return Ok();
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Authenticate a user",
            Description = "This endpoint allows a user to login by providing a valid username and password."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "If the user was authenticated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "If the username or password is missing or invalid.")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "If the username or password is incorrect.")]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new ApiException("Username and password are required.", (int)HttpStatusCode.BadRequest);
            }

            if (_authService.AuthenticateUser(username, password))
            {
                return Ok();
            }

            throw new ApiException("Invalid username or password.", (int)HttpStatusCode.Unauthorized);
        }

        [HttpGet("logout")]
        [SwaggerOperation(
            Summary = "Logout a user",
            Description = "This endpoint allows an authenticated user to logout."
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "If the user was logged out successfully.")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}