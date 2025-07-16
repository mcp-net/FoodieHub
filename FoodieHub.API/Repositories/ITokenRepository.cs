using FoodieHub.API.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace FoodieHub.API.Repositories
{
    public interface ITokenRepository
    {
        // OLD JWT Token Creation Method
        //string CreateJWTToken(IdentityUser user, List<string> roles);



        // JWE Token Creation Method
        //string CreateJWEToken(IdentityUser user, List<string> roles);


        (string AccessToken, string RefreshToken) CreateTokens(IdentityUser user, List<string> roles);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);


    }
}