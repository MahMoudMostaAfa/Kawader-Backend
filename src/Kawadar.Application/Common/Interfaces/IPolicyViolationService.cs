namespace Kawadar.Application.Common.Interfaces;

public interface IPolicyViolationService
{
  Task ProcessPolicyViolationAsync();
}