using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Neo4jClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KOET.Core.Services.Authentication.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;
        private readonly IGraphClient _client;


        public TokenService(IOptions<JwtSettings> settings, IGraphClient client)
        {
            _settings = settings.Value;
            _client = client;
        }

        public string GenerateToken1(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("session_id", Guid.NewGuid().ToString())
        };
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("session_id", Guid.NewGuid().ToString())
            };

            // Fetch all roles from Neo4j (direct + inherited)
            var roles = GetAllRolesAsync(user.Id).GetAwaiter().GetResult();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _settings.Issuer,
                ValidAudience = _settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key))
            };

            return tokenHandler.ValidateToken(token, parameters, out _);
        }

        private async Task<List<string>> GetAllRolesAsync(string userId)
        {
            var result = await _client.Cypher
                .Match("(u:User)-[:HAS_ROLE]->(r:Role)")
                .Where((User u) => u.Id == userId)
                .OptionalMatch("(r)-[:RELATED_TO*0..]->(inherited:Role)")
                .ReturnDistinct(inherited => inherited.As<Role>().Name)
                .ResultsAsync;

            return result.Distinct().ToList();
        }

    }
}
