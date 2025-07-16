using FoodieHub.API.Data;
using FoodieHub.API.Models.Domain;
using Jose;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FoodieHub.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _config;
        private readonly FoodieHubAuthDbContext _db;

        public TokenRepository(IConfiguration config, FoodieHubAuthDbContext db)
        {
            _config = config;
            _db = db;
        }

        public (string AccessToken, string RefreshToken) CreateTokens(
            IdentityUser user, List<string> roles)
        {
            // 1. Build JWE access token (as before)
            var payload = new Dictionary<string, object>
            {
                ["email"] = user.Email!,
                ["roles"] = roles,
                ["exp"] = DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds(),
                ["iss"] = _config["JWT:Issuer"],
                ["aud"] = _config["JWT:Audience"]
            };
            var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]!);
            var accessToken = Jose.JWT.Encode(
                payload, key, JweAlgorithm.DIR, JweEncryption.A256GCM);

            // 2. Generate opaque refresh token
            var refreshToken = Guid.NewGuid().ToString("N");
            var entity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            _db.RefreshTokens.Add(entity);
            _db.SaveChanges();

            return (accessToken, refreshToken);
        }

        public Task<RefreshToken?> GetRefreshTokenAsync(string token) =>
            _db.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var entity = await GetRefreshTokenAsync(token);
            if (entity != null)
            {
                entity.IsRevoked = true;
                await _db.SaveChangesAsync();
            }
        }
    }



    //public class TokenRepository : ITokenRepository
    //{
    //    private readonly IConfiguration _configuration;

    //    public TokenRepository(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //    }

    //    //JWE
    //    public string CreateJWEToken(IdentityUser user, List<string> roles)
    //    {
    //        var payload = new Dictionary<string, object>
    //        {
    //            { "email", user.Email },
    //            { "roles", roles },
    //            { "exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds() },
    //            { "iss", _configuration["JWT:Issuer"] },
    //            { "aud", _configuration["JWT:Audience"] }
    //        };

    //        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

    //        // Encrypt using AES GCM with RSA-OAEP key wrapping (or use direct symmetric encryption)
    //        string token = JWT.Encode(payload, key, JweAlgorithm.DIR, JweEncryption.A256GCM);

    //        return token;
    //    }


    //    ///////   OLD JWT Token Creation Method
    //    //////
    //    //////public string CreateJWTToken(IdentityUser user, List<string> roles)
    //    //////{
    //    //////    // Create claims
    //    //////    var claims = new List<Claim>();

    //    //////    claims.Add(new Claim(ClaimTypes.Email, user.Email));

    //    //////    foreach (var role in roles)
    //    //////    {
    //    //////        claims.Add(new Claim(ClaimTypes.Role, role));
    //    //////    }

    //    //////    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
    //    //////    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    //////    var token = new JwtSecurityToken(
    //    //////        _configuration["JWT:Issuer"],
    //    //////        _configuration["JWT:Audience"],
    //    //////        claims,
    //    //////        expires: DateTime.Now.AddMinutes(15),
    //    //////        signingCredentials: credentials);

    //    //////    return new JwtSecurityTokenHandler().WriteToken(token);
    //    //////}




    //}
}