using Kawadar.Domain.Specilizations;

namespace Kawadar.Tests.Common.Specilizations;

public static class SpecilizationFactory
{
    public static Specilization CreateActive(string name = "Backend Development")
    {
        var result = Specilization.Create(name, isActive: true);
        if (result.IsError)
            throw new InvalidOperationException($"Could not build Specilization: {result.TopError.Code} - {result.TopError.Description}");
        return result.Value;
    }

    public static Specilization CreateInactive(string name = "Legacy Systems")
    {
        var result = Specilization.Create(name, isActive: false);
        if (result.IsError)
            throw new InvalidOperationException($"Could not build Specilization: {result.TopError.Code} - {result.TopError.Description}");
        return result.Value;
    }
}
