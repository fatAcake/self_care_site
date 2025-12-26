using Microsoft.IdentityModel.Tokens;
using Self_care.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace Self_care.Services
{
    public class TokenService
    {
        private readonly JwtSettings _jwt;

        public TokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwt = jwtOptions.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("user_id", user.UserId.ToString()),
                new Claim("email", user.Email),
                new Claim("role_id", user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes > 0 ? _jwt.ExpiryMinutes : 60);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
