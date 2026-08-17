namespace Api.Extensions;

public static class RouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder WithDefaultProblems(this RouteHandlerBuilder builder) =>
        builder
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

    public static RouteHandlerBuilder WithValidationProblem(this RouteHandlerBuilder builder) =>
        builder.ProducesProblem(StatusCodes.Status400BadRequest);

    public static RouteHandlerBuilder WithNotFoundProblem(this RouteHandlerBuilder builder) =>
        builder.ProducesProblem(StatusCodes.Status404NotFound);

    public static RouteHandlerBuilder WithConflictProblem(this RouteHandlerBuilder builder) =>
        builder.ProducesProblem(StatusCodes.Status409Conflict);

    public static RouteHandlerBuilder WithAnonymousAuthProblems(this RouteHandlerBuilder builder) =>
        builder.ProducesProblem(StatusCodes.Status401Unauthorized);
}
