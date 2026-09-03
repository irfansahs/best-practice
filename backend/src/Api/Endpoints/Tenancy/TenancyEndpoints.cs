using Api.Extensions;
using Application.Abstractions.Messaging;
using Application.Contracts;
using Application.Security;
using Application.Tenancy.Features.Members.Commands.AddMember;
using Application.Tenancy.Features.Members.Commands.ChangeMemberStatus;
using Application.Tenancy.Features.Members.Commands.UpdateMemberRoles;
using Application.Tenancy.Features.Members.Queries.GetMembers;
using Application.Tenancy.Features.Organizations.Commands.ChangeOrganizationStatus;
using Application.Tenancy.Features.Organizations.Commands.CreateOrganization;
using Application.Tenancy.Features.Organizations.Commands.UpdateOrganization;
using Application.Tenancy.Features.Organizations.Queries.GetOrganizationById;
using Application.Tenancy.Features.Organizations.Queries.GetOrganizations;
using Application.Tenancy.Features.Permissions.Queries.GetPermissionCatalog;
using Application.Tenancy.Features.Roles.Commands.CreateRole;
using Application.Tenancy.Features.Roles.Commands.UpdateRolePermissions;
using Application.Tenancy.Features.Roles.Queries.GetRoles;

namespace Api.Endpoints.Tenancy;

public sealed class TenancyEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var orgs = app.MapGroup("/api/v1/tenancy/organizations").WithTags("Tenancy");
        orgs.MapGet("/", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetOrganizationsQuery(), ctx, ct))
            .AsQuery<IReadOnlyList<OrganizationListItemDto>>("GetOrganizations", Permissions.Tenancy.Organizations.Read);

        orgs.MapGet("/{id:guid}", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetOrganizationByIdQuery(id), ctx, ct))
            .AsGetById<OrganizationDetailDto>("GetOrganizationById", Permissions.Tenancy.Organizations.Read);

        orgs.MapPost("/", (CreateOrganizationCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/tenancy/organizations/{r.Id}"))
            .AsCreate<CreateOrganizationResponse>("CreateOrganization", Permissions.Tenancy.Organizations.Create);

        orgs.MapPut("/{id:guid}", (Guid id, UpdateOrganizationCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .AsUpdate("UpdateOrganization", Permissions.Tenancy.Organizations.Update);

        orgs.MapPost("/{id:guid}/status", (Guid id, ChangeOrganizationStatusCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { Id = id }, ctx, ct))
            .AsUpdate("ChangeOrganizationStatus", Permissions.Tenancy.Organizations.Update);

        orgs.MapGet("/{id:guid}/members", (Guid id, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetMembersQuery(id), ctx, ct))
            .AsQuery<IReadOnlyList<MemberListItemDto>>("GetMembers", Permissions.Tenancy.Members.Read);

        orgs.MapPost("/{id:guid}/members", (Guid id, AddMemberCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd with { OrganizationId = id }, ctx, ct, r => $"/api/v1/tenancy/organizations/{id}/members"))
            .AsCreate<AddMemberResponse>("AddMember", Permissions.Tenancy.Members.Manage);

        var members = app.MapGroup("/api/v1/tenancy/members").WithTags("Tenancy");
        members.MapPut("/{membershipId:guid}/roles", (Guid membershipId, UpdateMemberRolesCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { MembershipId = membershipId }, ctx, ct))
            .AsUpdate("UpdateMemberRoles", Permissions.Tenancy.Members.Manage);

        members.MapPost("/{membershipId:guid}/status", (Guid membershipId, ChangeMemberStatusCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { MembershipId = membershipId }, ctx, ct))
            .AsUpdate("ChangeMemberStatus", Permissions.Tenancy.Members.Manage);

        var roles = app.MapGroup("/api/v1/tenancy/roles").WithTags("Tenancy");
        roles.MapGet("/", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetRolesQuery(), ctx, ct))
            .AsQuery<IReadOnlyList<RoleListItemDto>>("GetRoles", Permissions.Tenancy.Roles.Read);

        roles.MapPost("/", (CreateRoleCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToCreated(cmd, ctx, ct, r => $"/api/v1/tenancy/roles/{r.Id}"))
            .AsCreate<CreateRoleResponse>("CreateRole", Permissions.Tenancy.Roles.Manage);

        roles.MapPut("/{id:guid}/permissions", (Guid id, UpdateRolePermissionsCommand cmd, IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToNoContent(cmd with { RoleId = id }, ctx, ct))
            .AsUpdate("UpdateRolePermissions", Permissions.Tenancy.Roles.Manage);

        app.MapGet("/api/v1/tenancy/permissions", (IDispatcher d, HttpContext ctx, CancellationToken ct) =>
                d.SendToApiResult(new GetPermissionCatalogQuery(), ctx, ct))
            .WithTags("Tenancy")
            .AsQuery<IReadOnlyList<PermissionCatalogItemDto>>("GetPermissionCatalog", Permissions.Tenancy.PermissionsCatalog.Read);
    }
}
