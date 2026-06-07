namespace Kawadar.Domain.Common.Results;

public readonly record struct Error
{

  private Error(string code, string description, ErrorKind kind)
  {
    Code = code;
    Description = description;
    Type = kind;
  }
  public string Code { get; init; }
  public string Description { get; init; }
  public ErrorKind Type { get; init; }

  public static Error Create(string code, string description, ErrorKind kind) => new(code, description, kind);


  public static Error Failure(string code = nameof(Failure), string description = "Genaral failure occurred.") =>
    new(code, description, ErrorKind.Failure);

  public static Error Unexpected(string code = nameof(Unexpected), string description = "An unexpected error occurred.") =>
    new(code, description, ErrorKind.Unexpected);

  public static Error Validation(string code = nameof(Validation), string description = "One or more validation errors occurred.") =>
    new(code, description, ErrorKind.Validation);
  public static Error NotFound(string code = nameof(NotFound), string description = "The requested resource was not found.") =>
    new(code, description, ErrorKind.NotFound);
  public static Error Conflict(string code = nameof(Conflict), string description = "A conflict occurred with the current state of the resource.") =>
    new(code, description, ErrorKind.Conflict);

  public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Authentication is required and has failed or has not yet been provided.") =>
    new(code, description, ErrorKind.Unauthorized);
  public static Error Forbidden(string code = nameof(Forbidden), string description = "You do not have permission to access the requested resource.") =>
    new(code, description, ErrorKind.Forbidden);


}