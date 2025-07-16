using Jose;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace FoodieHub.API.Authentication;
public class JweAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _configuration;

    public JweAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
            return Task.FromResult(AuthenticateResult.NoResult());

        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var json = JWT.Decode(token, key, JweAlgorithm.DIR, JweEncryption.A256GCM);
            var claimsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, claimsDict["email"].ToString())
            };

            if (claimsDict.TryGetValue("roles", out var rolesObj) && rolesObj is JsonElement rolesElement)
            {
                foreach (var role in rolesElement.EnumerateArray())
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.GetString()));
                }
            }

            var identity = new ClaimsIdentity(claims, JweAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, JweAuthenticationDefaults.AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception ex)
        {
            Logger.LogWarning("JWE token validation failed: {Message}", ex.Message);
            return Task.FromResult(AuthenticateResult.Fail("Invalid JWE token"));
        }
    }
}
