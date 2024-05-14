using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using RestAPi_BlogEngine_TestAoniken.Models;
using RestAPi_BlogEngine_TestAoniken.Repositories;
using System.Security.Claims;

namespace RestAPi_BlogEngine_TestAoniken.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public AuthService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public bool AuthenticateUser(string username, string password)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user != null && user.Password == password)
            {
                SetAuthenticationCookie(user.Username, user.Role);
                return true;
            }

            return false;
        }

        private void SetAuthenticationCookie(string username, string role)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(7)
                };

                _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            }
        }

        public int GetUserId(string username)
        {
            var user = _userRepository.GetUserByUsername(username);

            if (user != null && user.Role == "Writer")
            {
                return user.Id;
            }

            return 0;
        }
    }
}