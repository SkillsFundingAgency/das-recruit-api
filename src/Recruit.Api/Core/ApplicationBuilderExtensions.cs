namespace SFA.DAS.Recruit.Api.Core;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseScalar(this IApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/scalar"))
            {
                context.Response.Headers.ContentSecurityPolicy = "style-src 'self' 'unsafe-inline'; ";
            }
            await next();
        });

        return applicationBuilder;
    }
}