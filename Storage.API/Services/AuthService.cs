using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Storage.API.Core;
using Storage.API.Core.Interfaces;
using Storage.API.Models;

namespace Storage.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ServiceResult<LoginResponse>> Login(LoginRequest login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username ?? "", login.Password ?? "", isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded) return new ServiceResult<LoginResponse>(InvalidLoginAttempt);
            var user = await _userManager.FindByNameAsync(login.Username);
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var response = new LoginResponse()
            {
                Role = roles.FirstOrDefault(),
                Username = user.UserName,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = ((DateTimeOffset)token.ValidTo).ToUnixTimeSeconds(),
            };
            return new ServiceResult<LoginResponse>(value: response);
        }

        private JwtSecurityToken GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_jwtOptions.ExpireDays)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
        
        private static readonly ServiceResultError InvalidLoginAttempt = new ServiceResultError("INVALID_LOGIN_ATTEMPT");
    }


    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public int ExpireDays { get; set; }
    }
}
