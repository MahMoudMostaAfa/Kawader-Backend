using System.Security.Claims;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace Kawadar.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
  private readonly UserManager<AppUser> _userManager;
  private readonly SignInManager<AppUser> _signInManager;

  public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
  {
    _userManager = userManager;
    _signInManager = signInManager;
  }

  public async Task<Result<UserDto>> RegisterAsync(string email, string userName, string password)
  {
    var existingUser = await _userManager.FindByEmailAsync(email);
    if (existingUser is not null)
    {
      return Error.Conflict("User.AlreadyExists", "A user with this email already exists.");
    }

    var user = new AppUser
    {
      Email = email,
      UserName = userName,
      EmailConfirmed = false
    };

    var result = await _userManager.CreateAsync(user, password);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return new UserDto
    {
      Id = user.Id,
      Email = user.Email!,
      UserName = user.UserName!,
      EmailConfirmed = user.EmailConfirmed
    };
  }

  public async Task<Result<UserDto>> LoginAsync(string email, string password)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user is null)
    {
      return Error.Unauthorized("User.InvalidCredentials", "Invalid email or password.");
    }

    if (!user.EmailConfirmed)
    {
      return Error.Forbidden("User.EmailNotConfirmed", "Email address is not confirmed.");
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
    if (!result.Succeeded)
    {
      return Error.Unauthorized("User.InvalidCredentials", "Invalid email or password.");
    }

    return new UserDto
    {
      Id = user.Id,
      Email = user.Email!,
      UserName = user.UserName!,
      EmailConfirmed = user.EmailConfirmed
    };
  }

  public async Task<Result<Success>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<Success>> ResetPasswordAsync(string userId, string token, string newPassword)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<string>> GeneratePasswordResetTokenAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    return token;
  }

  public async Task<Result<Success>> ConfirmEmailAsync(string userId, string token)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.ConfirmEmailAsync(user, token);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    return token;
  }

  public async Task<Result<Success>> UpdateUserAsync(string userId, string? email, string? userName)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    if (!string.IsNullOrWhiteSpace(email) && email != user.Email)
    {
      user.Email = email;
      user.EmailConfirmed = false;
    }

    if (!string.IsNullOrWhiteSpace(userName))
    {
      user.UserName = userName;
    }

    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<Success>> DeleteUserAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.DeleteAsync(user);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<UserDto>> GetUserByIdAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    return new UserDto
    {
      Id = user.Id,
      Email = user.Email!,
      UserName = user.UserName!,
      EmailConfirmed = user.EmailConfirmed
    };
  }

  public async Task<Result<UserDto>> GetUserByEmailAsync(string email)
  {
    var user = await _userManager.FindByEmailAsync(email);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    return new UserDto
    {
      Id = user.Id,
      Email = user.Email!,
      UserName = user.UserName!,
      EmailConfirmed = user.EmailConfirmed
    };
  }

  public async Task<Result<bool>> IsInRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var isInRole = await _userManager.IsInRoleAsync(user, role);
    return isInRole;
  }

  public async Task<Result<Success>> AddToRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.AddToRoleAsync(user, role);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<Success>> RemoveFromRoleAsync(string userId, string role)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.RemoveFromRoleAsync(user, role);
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<List<string>>> GetUserRolesAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var roles = await _userManager.GetRolesAsync(user);
    return roles.ToList();
  }

  public async Task<Result<Success>> AddClaimAsync(string userId, string claimType, string claimValue)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<Success>> RemoveClaimAsync(string userId, string claimType, string claimValue)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var result = await _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
    if (!result.Succeeded)
    {
      var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
      return errors;
    }

    return Result.Success;
  }

  public async Task<Result<List<(string Type, string Value)>>> GetUserClaimsAsync(string userId)
  {
    var user = await _userManager.FindByIdAsync(userId);
    if (user is null)
    {
      return Error.NotFound("User.NotFound", "User not found.");
    }

    var claims = await _userManager.GetClaimsAsync(user);
    return claims.Select(c => (c.Type, c.Value)).ToList();
  }
}