using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
}