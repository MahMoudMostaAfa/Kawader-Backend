using Kawadar.Domain.UserProfiles.Enums;

public class UserProfileDto
{
  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;

  public string? Title { get; set; } = string.Empty;
  public Guid? specilizationId { get; set; }

  public string? Bio { get; set; } = string.Empty;
  public ExperienceYear ExperienceYear { get; set; } = ExperienceYear.LessThanOneYear;

  public string? ProfilePictureUrl { get; set; } = string.Empty;
  public string? VideoLink { get; set; } = string.Empty;
  public string? PhoneNumber { get; set; } = string.Empty;
  // availability Status
  public bool IsAvailable { get; set; } = true;



  // activation Status
  public bool IsActivated { get; set; } = false;
  public DateTime? ActivatedAt { get; set; }

  // Online Status
  public bool IsOnline { get; set; } = false;

  // Identity Verification
  public bool? IsIdentityVerified { get; set; } = false;

  public string UserName { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;

  // profile type 


  public ProfileType ProfileType { get; set; }




}