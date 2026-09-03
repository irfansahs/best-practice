using Domain.Identity;
using SharedKernel.Results;

namespace Application.Identity;

public static class ClientTypeParser
{
    public static Result<ClientType> Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Equals("web", StringComparison.OrdinalIgnoreCase))
            return ClientType.Web;
        if (value.Equals("mobile", StringComparison.OrdinalIgnoreCase))
            return ClientType.Mobile;
        return IdentityErrors.ClientTypeRequired;
    }
}
