namespace Mvp.Project.MvpSite.Sitemap.Providers
{
    internal class Common
    {
        internal static DotnetSitemapGenerator.ChangeFrequency GetChangeFrequency(string changefreq)
        {
            if (string.IsNullOrWhiteSpace(changefreq)) { return DotnetSitemapGenerator.ChangeFrequency.Never; }

            switch (changefreq.ToLowerInvariant())
            {
                case "always":
                    return DotnetSitemapGenerator.ChangeFrequency.Always;
                case "daily":
                    return DotnetSitemapGenerator.ChangeFrequency.Daily;
                case "weekly":
                    return DotnetSitemapGenerator.ChangeFrequency.Weekly;
                case "yearly":
                    return DotnetSitemapGenerator.ChangeFrequency.Yearly;
                case "hourly":
                    return DotnetSitemapGenerator.ChangeFrequency.Hourly;
                case "never":
                    return DotnetSitemapGenerator.ChangeFrequency.Never;
            }

            return DotnetSitemapGenerator.ChangeFrequency.Never;
        }
    }
}
