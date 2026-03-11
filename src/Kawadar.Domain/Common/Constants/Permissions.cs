namespace Kawadar.Domain.Common.Constants;

public static class Permissions
{
  // User Management
  public const string ViewUsers = "Permissions.Users.View";
  public const string CreateUsers = "Permissions.Users.Create";
  public const string EditUsers = "Permissions.Users.Edit";
  public const string DeleteUsers = "Permissions.Users.Delete";
  public const string ApproveUsers = "Permissions.Users.Approve";
  public const string BanUsers = "Permissions.Users.Ban";

  //Admins Management
  public const string ViewAdmins = "Permissions.Admins.View";
  public const string AddAdmin = "Permissions.Admins.Add";
  public const string AddClaim = "Permissions.Admins.AddClaim";


  // Job Management

  public const string DeleteJobs = "Permissions.Jobs.Delete";

  // Role Management
  public const string ViewRoles = "Permissions.Roles.View";
  public const string CreateRoles = "Permissions.Roles.Create";
  public const string EditRoles = "Permissions.Roles.Edit";
  public const string DeleteRoles = "Permissions.Roles.Delete";

  //Badge Managemet
  public const string ViewBadges = "Permissions.Badges.View";
  public const string DeleteBadges = "Permissions.Badges.Delete";
  public const string CreateBadges = "Permissions.Badges.Create";
  public const string EditBadges = "Permissions.Badges.Edit";



  public static List<string> GetAllPermissions()
  {
    return typeof(Permissions)
            .GetFields()
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)?.ToString() ?? string.Empty)
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();
  }
};
