using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeIdentityService : IIdentityService
{
    private readonly Dictionary<string, UserDto> _users = new();
    private readonly Dictionary<string, string> _passwords = new();
    private readonly Dictionary<string, HashSet<string>> _roles = new();
    private readonly Dictionary<string, List<(string Type, string Value)>> _claims = new();
    private readonly Dictionary<string, bool> _emailConfirmed = new();

    // Token stores: maps userId → issued token
    private readonly Dictionary<string, string> _passwordResetTokens = new();
    private readonly Dictionary<string, string> _emailConfirmationTokens = new();

    public Task<Result<UserDto>> RegisterAsync(string email, string userName, string password)
    {
        // Reject duplicate e-mail
        if (_users.Values.Any(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult<Result<UserDto>>(
                Error.Conflict("Identity.DuplicateEmail", $"Email '{email}' is already taken."));

        // Reject duplicate username
        if (_users.Values.Any(u => string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult<Result<UserDto>>(
                Error.Conflict("Identity.DuplicateUserName", $"Username '{userName}' is already taken."));

        var id = Guid.NewGuid().ToString();
        var user = new UserDto { Id = id, Email = email, UserName = userName, EmailConfirmed = false };
        _users[id] = user;
        _passwords[id] = password;
        _roles[id] = new HashSet<string>();
        _claims[id] = new List<(string, string)>();
        _emailConfirmed[id] = false;
        return Task.FromResult<Result<UserDto>>(user);
    }

    public Task<Result<UserDto>> LoginAsync(string email, string password)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email == email);
        if (user is null)
            return Task.FromResult<Result<UserDto>>(Error.NotFound("User.NotFound", $"User with email '{email}' not found."));

        if (_passwords.TryGetValue(user.Id, out var storedPassword) && storedPassword != password)
            return Task.FromResult<Result<UserDto>>(Error.Unauthorized("User.InvalidCredentials", "Invalid credentials."));

        return Task.FromResult<Result<UserDto>>(user);
    }

    public Task<Result<Success>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        if (_passwords.TryGetValue(userId, out var stored) && stored != currentPassword)
            return Task.FromResult<Result<Success>>(Error.Unauthorized("User.InvalidPassword", "Current password is incorrect."));

        _passwords[userId] = newPassword;
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> ResetPasswordAsync(string userId, string token, string newPassword)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        if (!_passwordResetTokens.TryGetValue(userId, out var issued) || issued != token)
            return Task.FromResult<Result<Success>>(
                Error.Unauthorized("Identity.InvalidToken", "Password reset token is invalid or expired."));

        _passwords[userId] = newPassword;
        _passwordResetTokens.Remove(userId); // tokens are one-time-use
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<string>> GeneratePasswordResetTokenAsync(string userId)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<string>>(Error.NotFound("User.NotFound", "User not found."));

        var token = $"reset-{userId}-{Guid.NewGuid():N}";
        _passwordResetTokens[userId] = token;
        return Task.FromResult<Result<string>>(token);
    }

    public Task<Result<Success>> ConfirmEmailAsync(string userId, string token)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        if (!_emailConfirmationTokens.TryGetValue(userId, out var issued) || issued != token)
            return Task.FromResult<Result<Success>>(
                Error.Unauthorized("Identity.InvalidToken", "Email confirmation token is invalid or expired."));

        _emailConfirmed[userId] = true;
        _emailConfirmationTokens.Remove(userId); // tokens are one-time-use
        _users[userId] = new UserDto
        {
            Id = _users[userId].Id,
            Email = _users[userId].Email,
            UserName = _users[userId].UserName,
            EmailConfirmed = true
        };
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<string>>(Error.NotFound("User.NotFound", "User not found."));

        var token = $"confirm-{userId}-{Guid.NewGuid():N}";
        _emailConfirmationTokens[userId] = token;
        return Task.FromResult<Result<string>>(token);
    }

    public Task<Result<Success>> UpdateUserAsync(string userId, string? email, string? userName)
    {
        if (!_users.TryGetValue(userId, out var user))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        _users[userId] = new UserDto
        {
            Id = user.Id,
            Email = email ?? user.Email,
            UserName = userName ?? user.UserName,
            EmailConfirmed = user.EmailConfirmed
        };
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> DeleteUserAsync(string userId)
    {
        if (!_users.ContainsKey(userId))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        _users.Remove(userId);
        _passwords.Remove(userId);
        _roles.Remove(userId);
        _claims.Remove(userId);
        _emailConfirmed.Remove(userId);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<UserDto>> GetUserByIdAsync(string userId)
    {
        if (_users.TryGetValue(userId, out var user))
            return Task.FromResult<Result<UserDto>>(user);

        return Task.FromResult<Result<UserDto>>(Error.NotFound("User.NotFound", $"User '{userId}' not found."));
    }

    public Task<Result<UserDto>> GetUserByEmailAsync(string email)
    {
        var user = _users.Values.FirstOrDefault(u => u.Email == email);
        if (user is not null)
            return Task.FromResult<Result<UserDto>>(user);

        return Task.FromResult<Result<UserDto>>(Error.NotFound("User.NotFound", $"User with email '{email}' not found."));
    }

    public Task<Result<bool>> IsInRoleAsync(string userId, string role)
    {
        if (!_roles.TryGetValue(userId, out var roles))
            return Task.FromResult<Result<bool>>(Error.NotFound("User.NotFound", "User not found."));

        return Task.FromResult<Result<bool>>(roles.Contains(role));
    }

    public Task<Result<Success>> AddToRoleAsync(string userId, string role)
    {
        if (!_roles.TryGetValue(userId, out var roles))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        roles.Add(role);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> RemoveFromRoleAsync(string userId, string role)
    {
        if (!_roles.TryGetValue(userId, out var roles))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        roles.Remove(role);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<List<string>>> GetUserRolesAsync(string userId)
    {
        if (!_roles.TryGetValue(userId, out var roles))
            return Task.FromResult<Result<List<string>>>(Error.NotFound("User.NotFound", "User not found."));

        return Task.FromResult<Result<List<string>>>(roles.ToList());
    }

    public Task<Result<Success>> AddClaimAsync(string userId, string claimType, string claimValue)
    {
        if (!_claims.TryGetValue(userId, out var claims))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        claims.Add((claimType, claimValue));
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<Success>> RemoveClaimAsync(string userId, string claimType, string claimValue)
    {
        if (!_claims.TryGetValue(userId, out var claims))
            return Task.FromResult<Result<Success>>(Error.NotFound("User.NotFound", "User not found."));

        claims.RemoveAll(c => c.Type == claimType && c.Value == claimValue);
        return Task.FromResult<Result<Success>>(Result.Success);
    }

    public Task<Result<List<(string Type, string Value)>>> GetUserClaimsAsync(string userId)
    {
        if (!_claims.TryGetValue(userId, out var claims))
            return Task.FromResult<Result<List<(string Type, string Value)>>>(Error.NotFound("User.NotFound", "User not found."));

        return Task.FromResult<Result<List<(string Type, string Value)>>>(claims.ToList());
    }

    public Task<Result<bool>> IsAvailableUserNameAsync(string userName)
    {
        var exists = _users.Values.Any(u => u.UserName == userName);
        return Task.FromResult<Result<bool>>(!exists);
    }

    public Task<Result<UserDto>> GetUserByUserNameAsync(string userName)
    {
        var user = _users.Values.FirstOrDefault(u => u.UserName == userName);
        if (user is not null)
            return Task.FromResult<Result<UserDto>>(user);

        return Task.FromResult<Result<UserDto>>(Error.NotFound("User.NotFound", $"User with username '{userName}' not found."));
    }

    public Task<Result<string>> GenerateUserNameAsync(string firstName, string lastName)
    {
        var shortGuid = Guid.NewGuid().ToString()[..8];
        return Task.FromResult<Result<string>>($"{firstName}.{lastName}.{shortGuid}");
    }

    public async Task<Result<IEnumerable<UserDto>>> GetUsersByIds(IEnumerable<string> Ids)
    {
        await Task.CompletedTask;
        var idSet = Ids.ToHashSet();
        var users = _users.Values.Where(u => idSet.Contains(u.Id)).ToList();
        return users;
    }

    /// <summary>
    /// Seed a user directly into the in-memory store (useful for test setup).
    /// </summary>
    public UserDto SeedUser(string? id = null, string email = "test@test.com", string userName = "testuser", string password = "P@ss1234", bool emailConfirmed = false)
    {
        id ??= Guid.NewGuid().ToString();
        var user = new UserDto { Id = id, Email = email, UserName = userName, EmailConfirmed = emailConfirmed };
        _users[id] = user;
        _passwords[id] = password;
        _roles[id] = new HashSet<string>();
        _claims[id] = new List<(string, string)>();
        _emailConfirmed[id] = emailConfirmed;
        return user;
    }

    public void Clear()
    {
        _users.Clear();
        _passwords.Clear();
        _roles.Clear();
        _claims.Clear();
        _emailConfirmed.Clear();
        _passwordResetTokens.Clear();
        _emailConfirmationTokens.Clear();
    }
}
