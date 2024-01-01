using DotnetSitemapGenerator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mvp.Foundation.DataFetching.GraphQL;
using Mvp.Project.MvpSite.Middleware;
using Mvp.Project.MvpSite.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mvp.Project.MvpSite.Rendering;

public class MvpSitemapDataProvider : ISitemapUrlProvider
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MvpSitemapDataProvider> _logger;
    private readonly IGraphQLClientFactory _graphQLClientFactory;
    private readonly IGraphQLRequestBuilder _graphQLRequestBuilder;
    private readonly IConfiguration _configuration;

    public MvpSitemapDataProvider(
        LinkGenerator linkGenerator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MvpSitemapDataProvider> logger,
        IGraphQLClientFactory graphQLClientFactory,
        IGraphQLRequestBuilder graphQLRequestBuilder,
        IConfiguration configuration)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _graphQLClientFactory = graphQLClientFactory;
        _graphQLRequestBuilder = graphQLRequestBuilder;
        _configuration = configuration;
    }

    private static DotnetSitemapGenerator.ChangeFrequency GetChangeFrequency(string changefreq)
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

    private Urlset GetXmlString(List<Result> result)
    {
        var row = result.FirstOrDefault();        
        var sitemapxmlstring = row.SitemapXml.Value;

        XmlSerializer serializer = new(typeof(Urlset));
        using StringReader reader = new(sitemapxmlstring);
        Urlset sitemapxml = (Urlset)serializer.Deserialize(reader);

        return sitemapxml;
    }

    public Task<IReadOnlyCollection<SitemapNode>> GetNodes()
    {
        var nodes = new List<SitemapNode>();
        var result = GetSiteMapXmlData();

        Urlset sitemap=GetXmlString(result);

        foreach (var row in sitemap.Url)
        {
            var url = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host.Value + row.Loc;

            _logger.LogInformation("Adding product: {URL}", url);

            SitemapNode node = new(url)
            {
                LastModificationDate = DateTime.ParseExact(row.Lastmod,
                "yyyyMMdd'T'HHmmss'Z'",
                CultureInfo.InvariantCulture),
                Priority = Convert.ToDecimal(row.Priority),
                ChangeFrequency = GetChangeFrequency(row.Changefreq)
            };
            nodes.Add(node);
        }        

        return Task.FromResult<IReadOnlyCollection<SitemapNode>>(nodes);
    }

    private List<Result> GetSiteMapXmlData()
    {
        CustomGraphQlLayoutServiceHandler customGraphQlLayoutServiceHandler = new(_configuration, _graphQLRequestBuilder, _graphQLClientFactory);
        return customGraphQlLayoutServiceHandler.GetSitemapXmlData().Result; 
    }

    //private List<Result> GetSiteMap()
    //{
    //    CustomGraphQlLayoutServiceHandler customGraphQlLayoutServiceHandler = new(_configuration, _graphQLRequestBuilder, _graphQLClientFactory);
    //    return customGraphQlLayoutServiceHandler.GetSitemap().Result;
    //}
}