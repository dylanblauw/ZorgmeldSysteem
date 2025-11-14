using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZorgmeldSysteem.Domain.Entities;
using ZorgmeldSysteem.Domain.Enums;
using ZorgmeldSysteem.Domain.Configuration;

namespace ZorgmeldSysteem.Persistence.Services
{
    public class JwtService
    {
        
private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateToken(User user, List<int> companyIds)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim("UserLevel", ((int)user.UserLevel).ToString()),
                new Claim("FullName", user.FullName)
            };

            // Add company claims
            foreach (var companyId in companyIds)
            {
                claims.Add(new Claim("CompanyID", companyId.ToString()));
            }

            // Add role claim
            var role = user.UserLevel switch
            {
                UserLevel.Admin => "Admin",
                UserLevel.Manager => "Manager",
                UserLevel.Mechanic => "Mechanic", 
                UserLevel.Reporter => "Reporter", 
                _ => "Unknown"
            };
            claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}