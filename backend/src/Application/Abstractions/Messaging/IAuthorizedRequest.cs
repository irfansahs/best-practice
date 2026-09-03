using Domain.Identity;

namespace Application.Abstractions.Messaging;

public interface IAuthorizedRequest
{
    string Permission { get; }

    PermissionScope RequiredScope => PermissionScope.Organization;
}

public interface IScopedAuthorizedRequest : IAuthorizedRequest
{
    new PermissionScope RequiredScope { get; }
}
