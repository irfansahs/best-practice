using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

namespace Api.Extensions;

public static class CompressionSetup
{
    private const int MinimumCompressionSize = 1024;

    public static IServiceCollection AddResponseCompressionServices(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            [
                "application/json",
                "application/problem+json",
                "text/plain",
                "text/css",
                "application/javascript"
            ]);
        });

        services.AddOptions<ResponseCompressionOptions>()
            .Configure(options => options.GetType().GetProperty("MinimumCompressionSize")?.SetValue(options, MinimumCompressionSize));

        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

        return services;
    }
}
