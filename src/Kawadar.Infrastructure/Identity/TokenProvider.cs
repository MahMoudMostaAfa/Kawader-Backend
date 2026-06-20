using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Kawadar.Infrastructure.Identity;

public class TokenProvider : ITokenProvider
{
  private readonly UserManager<AppUser> _userManager;
  private readonly IConfiguration _configuration;
  public TokenProvider(UserManager<AppUser> userManager, IConfiguration configuration)
  {
    _userManager = userManager;
    _configuration = configuration;
  }

  public Result<string> GenerateRefreshTokenAsync()
  {
    var randomNumber = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
  }

  public async Task<Result<string>> GenerateTokenAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return Error.NotFound();

    var roles = await _userManager.GetRolesAsync(user);
    var userCliams = await _userManager.GetClaimsAsync(user);

    var jwtSettings = _configuration.GetSection("JwtSettings");
    var issuer = jwtSettings["Issuer"]!;
    var audience = jwtSettings["Audience"]!;
    var key = jwtSettings["Secret"]!;

    var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));

    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub ,user.Id),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
      new Claim(JwtRegisteredClaimNames.Email, user.Email!),
    };
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    claims.AddRange(userCliams);

    var descriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = expires,
      Issuer = issuer,
      Audience = audience,
      SigningCredentials = new SigningCredentials(
              new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
              SecurityAlgorithms.HmacSha256Signature),

    };

    var tokenHandler = new JwtSecurityTokenHandler();

    var securityToken = tokenHandler.CreateToken(descriptor);
    var token = tokenHandler.WriteToken(securityToken);

    return token;



  }

  public Result<string> GetUserIdFromToken(string token)
  {
    if (string.IsNullOrEmpty(token)) return Error.Validation(description: "Token is null or empty");

    try
    {
      var tokenHandler = new JwtSecurityTokenHandler();

      if (!tokenHandler.CanReadToken(token))
        return Error.Validation(description: "Invalid token format");

      var jwtToken = tokenHandler.ReadJwtToken(token);

      var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

      if (string.IsNullOrEmpty(userId))
        return Error.Validation(description: "User ID claim not found in token");

      return userId;
    }
    catch (Exception)
    {
      return Error.Validation(description: "Failed to read token");
    }
  }
}