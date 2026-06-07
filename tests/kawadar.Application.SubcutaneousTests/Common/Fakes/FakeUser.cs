using Kawadar.Application.Common.Interfaces.Auth;

namespace kawadar.Application.SubcutaneousTests.Common.Fakes;

public class FakeUser : IUser
{
    public string? Id { get; set; }
    public List<string> Claims { get; set; } = [];

    public void SetUser(string userId) => Id = userId;
    public void ClearUser() => Id = null;
}
