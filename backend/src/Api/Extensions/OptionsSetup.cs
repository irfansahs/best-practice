using System.ComponentModel.DataAnnotations;

namespace Api.Extensions;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    [Required]
    public string[] AllowedOrigins { get; init; } = [];
}

public static class OptionsSetup
{
    public static IServiceCollection AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
