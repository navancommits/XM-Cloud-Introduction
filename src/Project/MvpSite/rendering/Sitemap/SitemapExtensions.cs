using Microsoft.Extensions.DependencyInjection;

namespace Mvp.Project.MvpSite.Rendering;

public static class SitemapExtensions
{
    public static void AddSitemap(this IServiceCollection services)
    {
        // add sitemap services
        services.AddScoped<ISitemapProvider,MvpSitemapUrlProvider>();
        services.AddScoped<SitemapBuilder>();
    }
}