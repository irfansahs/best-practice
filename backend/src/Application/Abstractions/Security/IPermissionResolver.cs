using Domain.Identity;

namespace Application.Abstractions.Security;

public interface IPermissionResolver
{
    Task<PermissionSet> ResolveAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
}
